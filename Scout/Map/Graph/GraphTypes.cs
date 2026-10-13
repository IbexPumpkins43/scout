namespace Scout.Map.Graph;

internal readonly record struct GraphNode(
    MapPosition Position,
    int FirstEdge,
    int EdgeCount);

internal readonly record struct GraphEdge(
    int Target,
    float Distance);
