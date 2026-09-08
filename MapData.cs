namespace Scout;

internal class MapDataException : Exception
{
    public MapDataException(string path, string message) 
        : base($"{path} : {message}")
    {
    }

    public MapDataException(string path, string message, Exception innerException)
        : base($"{path} : {message}", innerException)
    {
    }
}

internal readonly record struct MapPosition(
    double X,
    double Y);

internal class MapData(string path)
{
    public string Path { get; } = path;
    public Dictionary<long, MapPosition> NodePositions { get; } = new();
    public List<OSMWay> Roads { get; } = new();

    public void Load()
    {
        using PBFReader pbfReader = new(path: this.Path);
        pbfReader.Open();

        OSMDecoder osmDecoder = new();
        
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
            throw new MapDataException(
                this.Path, 
                $"Expected header, but got {osmHeader.GetType().Name}");
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
                MapPosition mapPosition = new(
                    X: node.Coordinates.Latitude,
                    Y: node.Coordinates.Longitude);
                this.NodePositions.Add(node.Id, mapPosition);
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

        MapPosition origin = this.NodePositions.First().Value;
        double originX = origin.X;
        double originY = origin.Y;

        foreach (var (id, (x, y)) in this.NodePositions)
        {
            this.NodePositions[id] = new(
                // Use a local origin to keep projected coordinates near 0
                X: 
                    (y - originY) 
                    * Math.Cos(double.DegreesToRadians(originX))
                    * metresPerDegree,
                // Invert Y so north appears upward
                Y: -(x - originX) * metresPerDegree);
        }

    }
}
