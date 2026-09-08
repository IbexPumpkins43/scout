namespace Scout;

internal class MapDataException : Exception
{
    public MapDataException(string path, string message) 
        : base($"{path} : {message}")
    {}

    public MapDataException(string path, string message, Exception innerException)
        : base($"{path} : {message}", innerException)
    {}
}

internal class MapData(string path)
{
    public string Path { get; private set; } = path;
    public Dictionary<long, OSMCoordinates> NodePositions { get; private set; } = new Dictionary<long, OSMCoordinates>();
    public List<OSMWay> Roads { get; private set; } = new();

    public void Load()
    {
        using PBFReader pbfReader = new PBFReader(Path);
        pbfReader.Open();

        OSMDecoder osmDecoder = new OSMDecoder();
        
        this.ValidateHeader(pbfReader, osmDecoder);
        this.DecodeBlocks(pbfReader, osmDecoder); 
        this.ProjectNodes();
    }

    private void ValidateHeader(PBFReader pbfReader, OSMDecoder osmDecoder) 
    {
        PBFBlock? pbfHeader = pbfReader.ReadNext();
        if (pbfHeader == null)
        {
            throw new MapDataException(this.Path, "Header is missing");
        }
        
        OSMBlock osmHeader = osmDecoder.Parse(pbfHeader);
        if (osmHeader.GetType() != typeof(OSMHeaderBlock))
        {
            throw new MapDataException(this.Path, $"Expected header, but got {"TODO"}");
        }
    }

    private void DecodeBlocks(PBFReader pbfReader, OSMDecoder osmDecoder) 
    {
        PBFBlock? pbfBlock = pbfReader.ReadNext();
        while (pbfBlock != null)
        {
            OSMDataBlock osmBlock = (OSMDataBlock)osmDecoder.Parse(pbfBlock);

            foreach (OSMNode node in osmDecoder.DecodeNodes(osmBlock))
            {
                this.NodePositions.Add(node.ID, node.Coordinates);
            }

            foreach (OSMWay way in osmDecoder.DecodeWays(osmBlock))
            {
                if (way.Tags.ContainsKey("highway"))
                {
                    this.Roads.Add(way);
                }
            }
                        
            pbfBlock = pbfReader.ReadNext();
        }
    }

    private void ProjectNodes() 
    {
        const int metresPerDegree = 111320;

        if (this.NodePositions.Count == 0)
        {
            throw new MapDataException(this.Path, "Map contains no nodes");
        }

        OSMCoordinates origin = this.NodePositions.First().Value;
        double originLatitude = origin.Latitude;
        double originLongitude = origin.Longitude;

        foreach (var (id, (latitude, longitude)) in this.NodePositions)
        {
            this.NodePositions[id] = new OSMCoordinates(
                // Use a local origin to keep projected coordinates near 0
                Latitude: 
                    (longitude - originLongitude) 
                    * Math.Cos(double.DegreesToRadians(originLatitude))
                    * metresPerDegree,
                // Invert Y so north appears upward
                Longitude: -(latitude - originLatitude) * metresPerDegree);
        }

    }
}
