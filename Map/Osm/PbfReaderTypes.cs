namespace Scout.Map.Osm;

internal enum PBFBlockType
{
    OsmHeader,
    OsmData
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
