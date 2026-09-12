using Google.Protobuf;

namespace Scout.Map;

internal enum PBFBlockType
{
    OSMHeader,
    OSMData
}

internal record PBFBlock(
    long Index,
    ReadOnlyMemory<byte> Bytes,
    PBFBlockType Type);

internal class PBFReaderException : Exception
{
    public PBFReaderException(string path, string message)
        : base($"{path} : {message}")
    {
    }

    public PBFReaderException(string path, string message, Exception innerException)
        : base($"{path} : {message}", innerException)
    {
    }
}
