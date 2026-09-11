namespace Scout.Map;

internal readonly record struct GraphNode(
    MapPosition Position,
    int FirstEdge,
    int EdgeCount);

internal readonly record struct GraphEdge(
    int Target,
    float Distance);

internal class GraphData
{
    public GraphNode[] Nodes { get; private set; } = Array.Empty<GraphNode>();
    public GraphEdge[] Edges { get; private set; } = Array.Empty<GraphEdge>();

    public void Build(MapNodePositions nodePositions, MapRoads roads)
    {
        Dictionary<long, int> nodeIndices = BuildNodeIndices(nodePositions, roads);

        int[] edgeCounts = new int[nodeIndices.Count];
        int edgeCount = CountEdges(roads, nodeIndices, edgeCounts);

        GraphNode[] nodes = BuildNodes(nodePositions, nodeIndices, edgeCounts);

        GraphEdge[] edges = new GraphEdge[edgeCount];
        PrepareEdgeWriteOffsets(
            nodes,
            edgeCounts);
        BuildEdges(
            roads,
            nodeIndices,
            nodes,
            edges,
            edgeCounts);

        this.Nodes = nodes;
        this.Edges = edges;
    }

    private Dictionary<long, int> BuildNodeIndices(MapNodePositions nodePositions, MapRoads roads)
    {
        Dictionary<long, int> nodeIndices = new();

        foreach (OSMWay road in roads)
        {
            foreach (long nodeId in road.NodeIds)
            {
                if (!nodePositions.ContainsKey(nodeId))
                {
                    continue;
                }

                nodeIndices.TryAdd(nodeId, nodeIndices.Count);
            }
        }

        return nodeIndices;
    }

    private int CountEdges(MapRoads roads, Dictionary<long, int> nodeIndices, int[] edgeCounts)
    {
        int totalEdgeCount = 0;

        foreach (OSMWay road in roads)
        {
            for (int index = 0; index < road.NodeIds.Count - 1; index++)
            {
                long firstId = road.NodeIds[index];
                long secondId = road.NodeIds[index + 1];

                if (!TryGetNodeIndices(
                    nodeIndices,
                    firstId,
                    secondId,
                    out int firstIndex,
                    out int secondIndex))
                {
                    continue;
                }

                edgeCounts[firstIndex]++;
                edgeCounts[secondIndex]++;

                totalEdgeCount += 2;
            }
        }

        return totalEdgeCount;
    }

    private GraphNode[] BuildNodes(
        MapNodePositions nodePositions,
        Dictionary<long, int> nodeIndices,
        int[] edgeCounts)
    {
        GraphNode[] nodes = new GraphNode[nodeIndices.Count];

        foreach (KeyValuePair<long, int> entry in nodeIndices)
        {
            MapPosition position = nodePositions[entry.Key];

            nodes[entry.Value] = new(
                Position: position,
                FirstEdge: 0,
                EdgeCount: edgeCounts[entry.Value]);
        }

        int firstEdge = 0;

        for (int index = 0; index < nodes.Length; index++)
        {
            GraphNode node = nodes[index];

            nodes[index] = new(
                Position: node.Position,
                FirstEdge: firstEdge,
                EdgeCount: node.EdgeCount);

            firstEdge += node.EdgeCount;
        }

        return nodes;
    }

    private void PrepareEdgeWriteOffsets(GraphNode[] nodes, int[] edgeCounts)
    {
        for (int index = 0; index < nodes.Length; index++)
        {
            edgeCounts[index] = nodes[index].FirstEdge;
        }
    }

    private void BuildEdges(
        MapRoads roads,
        Dictionary<long, int> nodeIndices,
        GraphNode[] nodes,
        GraphEdge[] edges,
        int[] edgeOffsets)
    {
        foreach (OSMWay road in roads)
        {
            for (int index = 0; index < road.NodeIds.Count - 1; index++)
            {
                long firstId = road.NodeIds[index];
                long secondId = road.NodeIds[index + 1];

                if (!TryGetNodeIndices(
                    nodeIndices,
                    firstId,
                    secondId,
                    out int firstIndex,
                    out int secondIndex))
                {
                    continue;
                }

                float distance = CalculateDistance(
                    nodes[firstIndex].Position,
                    nodes[secondIndex].Position);

                int forwardEdgeIndex = edgeOffsets[firstIndex]++;
                int backwardEdgeIndex = edgeOffsets[secondIndex]++;

                edges[forwardEdgeIndex] = new(
                    Target: secondIndex,
                    Distance: distance);

                edges[backwardEdgeIndex] = new(
                    Target: firstIndex,
                    Distance: distance);
            }
        }
    }

    private bool TryGetNodeIndices(
        Dictionary<long, int> nodeIndices,
        long firstId,
        long secondId,
        out int firstIndex,
        out int secondIndex)
    {
        if (!nodeIndices.TryGetValue(firstId, out firstIndex))
        {
            secondIndex = 0;
            return false;
        }

        if (!nodeIndices.TryGetValue(secondId, out secondIndex))
        {
            return false;
        }

        return firstIndex != secondIndex;
    }

    private float CalculateDistance(MapPosition first, MapPosition second)
    {
        double deltaX = second.X - first.X;
        double deltaY = second.Y - first.Y;

        return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }
}

internal class Graph(GraphData graphData)
{
    private GraphData _graphData = graphData;

    private float[] _distances = new float[graphData.Nodes.Length];
    private int[] _previous = new int[graphData.Nodes.Length];
    
    private PriorityQueue<int, float> _queue = new();

    public int[] Dijkstras(int start, int target)
    {
        Array.Fill(this._distances, float.PositiveInfinity);
        Array.Fill(this._previous, -1);

        this._distances[start] = 0.0f;
        this._queue.Enqueue(start, 0.0f);

        while (this._queue.TryDequeue(out int current, out float currentDistance))
        {
            if (current == target)
            {
                return this.BuildPath(start, target);
            }

            if (currentDistance > this._distances[current])
            {
                continue;
            }

            GraphNode node = this._graphData.Nodes[current];
            int edgeEnd = node.FirstEdge + node.EdgeCount;

            for (int edgeIndex = node.FirstEdge; edgeIndex < edgeEnd; edgeIndex++)
            {
                GraphEdge edge = this._graphData.Edges[edgeIndex];

                float distance = this._distances[current] + edge.Distance;
                if (distance >= this._distances[edge.Target])
                {
                    continue;
                }

                this._distances[edge.Target] = distance;
                this._previous[edge.Target] = current;

                this._queue.Enqueue(
                    edge.Target,
                    distance);
            }
        }

        return [];
    }

    private int[] BuildPath(int start, int target)
    {
        int length = 1;
        for (int node = target; node != start; node = this._previous[node])
        {
            length++;
        }

        int[] path = new int[length];
        int current = target;
        for (int index = path.Length - 1; index >= 0; index--)
        {
            path[index] = current;
            current = this._previous[current];
        }

        return path;
    }
}
