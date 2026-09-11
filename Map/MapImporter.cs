global using MapNodePositions = System.Collections.Generic.Dictionary<long, Scout.Map.MapPosition>;
global using MapRoads = System.Collections.Generic.List<Scout.Map.OSMWay>;

namespace Scout.Map;

internal readonly record struct MapPosition(
    double X,
    double Y);

internal class MapData
{
    public RenderData RenderData { get; private set; } = new();
    public GraphData GraphData { get; private set; } = new();
}

internal class MapImporterException : Exception
{
    public MapImporterException(string path, string message) 
        : base($"{path} : {message}")
    {
    }

    public MapImporterException(string path, string message, Exception innerException)
        : base($"{path} : {message}", innerException)
    {
    }
}

internal class MapImporter(string path)
{
    private string _path = path;

    public MapData Import()
    {
        using PBFReader pbfReader = new(path: this._path);
        pbfReader.Open();

        OSMDecoder osmDecoder = new();
        
        this.ValidateHeader(pbfReader, osmDecoder);

        MapNodePositions nodePositions = new();
        MapRoads roads = new();
        this.DecodeBlocks(pbfReader, osmDecoder, nodePositions, roads); 
        this.ProjectNodes(nodePositions);

        MapData mapData = new();
        mapData.RenderData.Build(nodePositions, roads);
        mapData.GraphData.Build(nodePositions, roads);

        return mapData;
    }

    private void ValidateHeader(PBFReader pbfReader, OSMDecoder osmDecoder) 
    {
        PBFBlock? pbfHeader = pbfReader.ReadNext();
        if (pbfHeader == null)
        {
            throw new MapImporterException(this._path, "Header is missing");
        }
        
        OSMBlock osmHeader = osmDecoder.Parse(pbfHeader);
        if (osmHeader.GetType() != typeof(OSMHeaderBlock))
        {
            throw new MapImporterException(
                this._path, 
                $"Expected header, but got {osmHeader.GetType().Name}");
        }
    }

    private void DecodeBlocks(
        PBFReader pbfReader, 
        OSMDecoder osmDecoder,
        MapNodePositions nodePositions,
        MapRoads roads) 
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
                nodePositions.Add(node.Id, mapPosition);
            }

            foreach (OSMWay way in osmDecoder.DecodeWays(osmBlock))
            {
                if (way.Tags.ContainsKey("highway"))
                {
                    roads.Add(way);
                }
            }
                        
            pbfBlock = pbfReader.ReadNext();
        }
    }

    private void ProjectNodes(MapNodePositions nodePositions) 
    {
        const int metresPerDegree = 111320;

        if (nodePositions.Count == 0)
        {
            throw new MapImporterException(this._path, "Map contains no nodes");
        }

        MapPosition origin = nodePositions.First().Value;
        double originX = origin.X;
        double originY = origin.Y;

        foreach (var (id, (x, y)) in nodePositions)
        {
            nodePositions[id] = new(
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
