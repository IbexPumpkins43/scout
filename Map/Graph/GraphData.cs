namespace Scout.Map;

internal class GraphData
{
    public GraphNode[] Nodes { get; private set; } = Array.Empty<GraphNode>();
    public GraphEdge[] Edges { get; private set; } = Array.Empty<GraphEdge>();

    public void Build(MapNodePositions nodePositions, MapRoads roads)
    {
        Dictionary<long, int> nodeIndices = this.BuildNodeIndices(nodePositions, roads);

        int[] edgeCounts = new int[nodeIndices.Count];
        int edgeCount = this.CountEdges(roads, nodeIndices, edgeCounts);

        GraphNode[] nodes = this.BuildNodes(nodePositions, nodeIndices, edgeCounts);

        GraphEdge[] edges = new GraphEdge[edgeCount];
        this.PrepareEdgeWriteOffsets(
            nodes,
            edgeCounts);
        this.BuildEdges(
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
            for (int index = 0; index < road.NodeIds.Length - 1; index++)
            {
                long firstId = road.NodeIds[index];
                long secondId = road.NodeIds[index + 1];

                if (!this.TryGetNodeIndices(
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
            for (int index = 0; index < road.NodeIds.Length - 1; index++)
            {
                long firstId = road.NodeIds[index];
                long secondId = road.NodeIds[index + 1];

                if (!this.TryGetNodeIndices(
                    nodeIndices,
                    firstId,
                    secondId,
                    out int firstIndex,
                    out int secondIndex))
                {
                    continue;
                }

                float distance = this.CalculateDistance(
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