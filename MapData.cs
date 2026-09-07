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

internal class MapData
{
    public string Path { get; private set; }
    public Dictionary<long, OSMCoordinates> NodePositions { get; private set; }
    public List<OSMWay> Roads { get; private set; }

    public MapData(string path)
    {
        this.Path = path;
        this.NodePositions = new Dictionary<long, OSMCoordinates>();
        this.Roads = new List<OSMWay>();
    }

    public void Load()
    {
        using var pbfReader = new PBFReader(Path);
        pbfReader.Open();

        var osmDecoder = new OSMDecoder();
        
        // A valid OSM PBF must begin with an OSMHeader block
        var pbfHeader = pbfReader.ReadNext();
        if (pbfHeader == null)
        {
            throw new MapDataException(this.Path, "Header is missing");
        }
        
        var osmHeader = osmDecoder.Parse(pbfHeader);
        if (osmHeader.GetType() != typeof(OSMHeaderBlock))
        {
            throw new MapDataException(this.Path, $"Expected header, but got {"TODO"}");
        }

        // Decode blocks
        var pbfBlock = pbfReader.ReadNext();
        while (pbfBlock != null)
        {
            var osmBlock = (OSMDataBlock)osmDecoder.Parse(pbfBlock);

            foreach (var node in osmDecoder.DecodeNodes(osmBlock))
            {
                this.NodePositions.Add(node.ID, node.Coordinates);
            }

            foreach (var way in osmDecoder.DecodeWays(osmBlock))
            {
                if (way.Tags.ContainsKey("highway"))
                {
                    this.Roads.Add(way);
                }
            }
                        
            pbfBlock = pbfReader.ReadNext();
        }

        // Project nodes
        if (this.NodePositions.Count == 0)
        {
            throw new MapDataException(this.Path, "Map contains no nodes");
        }

        var (originLatitude, originLongitude) = this.NodePositions.First();
        var metresPerDegree = 111320;

        foreach (var (id, (latitude, longitude)) in this.NodePositions)
        {
            this.NodePositions[id] = new OSMCoordinates(
                // Use a local origin to keep projected coordinates near 0
                Latitude: (longitude - originLatitude) * Math.Cos(double.DegreesToRadians(originLatitude)) * metresPerDegree,
                // Invert Y so north appears upward
                Longitude: -(latitude - originLatitude) * metresPerDegree);
        }
    }
}
