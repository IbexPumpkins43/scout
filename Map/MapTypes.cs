global using MapNodePositions = System.Collections.Generic.Dictionary<long, Scout.Map.MapPosition>;
global using MapRoads = System.Collections.Generic.List<Scout.Map.OSMWay>;
global using MapPlaces = System.Collections.Generic.List<Scout.Map.OSMNode>;
global using MapBuildings = System.Collections.Generic.List<Scout.Map.OSMWay>;

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
