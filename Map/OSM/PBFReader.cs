using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Google.Protobuf;
using OSMPBF;

namespace Scout.Map;

internal class PBFReader(string path) : IDisposable
{
    private string _path = path;

    private Stream? _stream;
    private BinaryReader? _reader;

    private List<long> _blockOffsets = new();
    private bool _blockOffsetsComplete;
    private long _nextBlockIndex;
    private long _nextUndiscoveredBlockOffset;

    private bool _disposed;

    public void Open()
    {
        ObjectDisposedException.ThrowIf(this._disposed, this);

        if (this._stream == null)
        {
            this._stream = File.OpenRead(this._path);
        }

        if (this._reader == null)
        {
            this._reader = new(input: this._stream);
        }
    }

    public void Dispose()
    {
        if (this._disposed)
        {
            return;
        }

        if (this._reader != null)
        {
            this._reader.Dispose();
        }
        else if (this._stream != null)
        {
            this._stream.Dispose();
        }

        this._stream = null;
        this._reader = null;
        this._disposed = true;
    }

    public PBFBlock? ReadNext()
    {
        this.ValidateReader();

        if (this._stream.Position == this._stream.Length)
        {
            this._blockOffsetsComplete = true;
            return null;
        }

        long blockIndex = this._nextBlockIndex;
        long blockOffset = this._stream.Position;

        PBFBlock block = this.ReadBlock(this._stream, this._reader, blockIndex);

        // Cache offsets as blocks are discovered for later indexed access
        if (blockIndex == this._blockOffsets.Count)
        {
            this._blockOffsets.Add(blockOffset);
            this._nextUndiscoveredBlockOffset = this._stream.Position;
        }

        this._nextBlockIndex++;

        return block;
    }

    public PBFBlock ReadIndex(long blockIndex)
    {
        this.ValidateReader();

        // Lazily scan only as far as the requested block
        this.DiscoverOffsets(this._stream, this._reader, blockIndex);

        if (blockIndex >= this._blockOffsets.Count)
        {
            throw new PBFReaderException(this._path, $"Block index out of bounds: {blockIndex}");
        }

        long originalPosition = this._stream.Position;
        try
        {
            this._stream.Position = this._blockOffsets[(int)blockIndex];
            return this.ReadBlock(this._stream, this._reader, blockIndex);
        }
        finally
        {
            // Preserve the position used by sequential reads
            this._stream.Position = originalPosition;
        }
    }

    private PBFBlock ReadBlock(Stream stream, BinaryReader reader, long blockIndex)
    {
        try
        {
            // Each file block contains a length-prefixed header followed by its payload
            BlobHeader header = this.ReadHeader(reader);
            PBFBlockType blockType = this.GetBlockType(header);
            ReadOnlyMemory<byte> payload = this.ReadPayload(stream, reader, header);

            return new(
                Index: blockIndex,
                Bytes: payload,
                Type: blockType);
        }
        catch (Exception exception) when (
            exception is IOException
                or InvalidProtocolBufferException
                or InvalidDataException)
        {
            throw new PBFReaderException(
                this._path,
                $"Failed to read block: {blockIndex}",
                exception);
        }
    }

    private void ReadRequiredBytes(BinaryReader reader, Span<byte> buffer, string description)
    {
        try
        {
            reader.ReadExactly(buffer);
        }
        catch (EndOfStreamException eosException)
        {
            throw new PBFReaderException(this._path, $"Truncated {description}", eosException);
        }
    }

    private BlobHeader ReadHeader(BinaryReader reader)
    {
        const int headerSizeLength = sizeof(int);
        const int headerMaxSize = 64 * 1024;

        // BlobHeader length is stored as a four-byte big-endian integer so this needs doing
        Span<byte> headerSizeBytes = stackalloc byte[headerSizeLength];
        this.ReadRequiredBytes(reader, headerSizeBytes, "blob header length");

        int headerSize = BinaryPrimitives.ReadInt32BigEndian(headerSizeBytes);
        if (headerSize <= 0 || headerSize >= headerMaxSize)
        {
            throw new PBFReaderException(this._path, $"Invalid blob header size: {headerSize}");
        }

        byte[] headerData = new byte[headerSize];
        this.ReadRequiredBytes(reader, headerData, "blob header");

        return BlobHeader.Parser.ParseFrom(headerData);
    }

    private ReadOnlyMemory<byte> ReadPayload(Stream stream, BinaryReader reader, BlobHeader header)
    {
        // The header specifies the size of the serialized Blob
        int blobSize = header.Datasize;
        this.ValidateBlobSize(stream, blobSize);

        byte[] blobData = new byte[blobSize];
        this.ReadRequiredBytes(reader, blobData, "blob");

        Blob blob = Blob.Parser.ParseFrom(blobData);

        // Extract the payload according to the Blob's encoding
        if (blob.HasRaw)
        {
            this.ValidatePayloadSize(blob.Raw.Length);
            return blob.Raw.Memory;
        }

        if (!blob.HasZlibData)
        {
            throw new PBFReaderException(this._path, "Unsupported compression format");
        }

        if (!blob.HasRawSize)
        {
            throw new PBFReaderException(
                this._path,
                "Compressed blob is missing its raw size");
        }

        this.ValidatePayloadSize(blob.RawSize);

        // Attempt to get the underlying array of ZlibData, and on failure just copy it
        MemoryStream compressedStream;
        if (MemoryMarshal.TryGetArray(blob.ZlibData.Memory, out ArraySegment<byte> segment))
        {
            compressedStream = new(
                buffer: segment.Array!,
                index: segment.Offset,
                count: segment.Count,
                writable: false,
                publiclyVisible: false);
        }
        else
        {
            compressedStream = new(buffer: blob.ZlibData.ToByteArray());
        }

        using (compressedStream)
        using (ZLibStream zlibStream = new(
            stream: compressedStream,
            mode: CompressionMode.Decompress))
        {
            byte[] payloadBuffer = new byte[blob.RawSize];

            int payloadLength = zlibStream.ReadAtLeast(payloadBuffer, blob.RawSize, false);
            if (payloadLength != blob.RawSize || zlibStream.ReadByte() != -1)
            {
                throw new PBFReaderException(
                    this._path,
                    $"Invalid payload size: expected {blob.RawSize}");
            }

            return payloadBuffer;
        }
    }

    private void DiscoverOffsets(Stream stream, BinaryReader reader, long blockIndex)
    {
        if (blockIndex < 0)
        {
            throw new PBFReaderException(
                this._path,
                $"Block index out of bounds: {blockIndex}");
        }

        if (blockIndex < this._blockOffsets.Count || this._blockOffsetsComplete)
        {
            return;
        }

        long originalPosition = stream.Position;
        try
        {
            stream.Position = this._nextUndiscoveredBlockOffset;

            // Record block offsets while skipping over their payloads
            while (this._blockOffsets.Count <= blockIndex)
            {
                if (stream.Position == stream.Length)
                {
                    this._blockOffsetsComplete = true;
                    break;
                }
                if (stream.Position > stream.Length)
                {
                    throw new PBFReaderException(
                        this._path,
                        "Blob extends beyond the end of the file");
                }

                long blockOffset = stream.Position;
                BlobHeader header = this.ReadHeader(reader);

                int blobSize = header.Datasize;
                this.ValidateBlobSize(stream, blobSize);

                long nextOffset = stream.Position + blobSize;

                this._blockOffsets.Add(blockOffset);
                this._nextUndiscoveredBlockOffset = nextOffset;
                stream.Position = nextOffset;
            }

            if (this._nextUndiscoveredBlockOffset == stream.Length)
            {
                this._blockOffsetsComplete = true;
            }
        }
        finally
        {
            // Preserve the position used by sequential reads
            stream.Position = originalPosition;
        }
    }

    [MemberNotNull(nameof(_stream), nameof(_reader))]
    private void ValidateReader()
    {
        ObjectDisposedException.ThrowIf(this._disposed, this);

        if (this._stream == null || this._reader == null)
        {
            throw new PBFReaderException(this._path, "Call \"Open()\" before trying to read");
        }
    }

    private void ValidateBlobSize(Stream stream, int blobSize)
    {
        const int blobMaxSize = 32 * 1024 * 1024;

        if (blobSize <= 0 || blobSize >= blobMaxSize)
        {
            throw new PBFReaderException(this._path, $"Invalid blob size: {blobSize}");
        }

        if (blobSize > stream.Length - stream.Position)
        {
            throw new PBFReaderException(
                this._path,
                "Blob extends beyond the end of the file");
        }
    }

    private void ValidatePayloadSize(int payloadSize)
    {
        const int payloadMaxSize = 32 * 1024 * 1024;

        if (payloadSize < 0 || payloadSize >= payloadMaxSize)
        {
            throw new PBFReaderException(this._path, $"Invalid payload size: {payloadSize}");
        }
    }

    private PBFBlockType GetBlockType(BlobHeader header)
    {
        return header.Type switch
        {
            "OSMHeader" => PBFBlockType.OSMHeader,
            "OSMData" => PBFBlockType.OSMData,
            _ => throw new PBFReaderException(this._path, $"Invalid block type: {header.Type}")
        };
    }
}
