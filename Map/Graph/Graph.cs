namespace Scout.Map;

internal class Graph(GraphData graphData)
{
    private GraphData _graphData = graphData;

    private float[] _distances = new float[graphData.Nodes.Length];
    private int[] _previous = new int[graphData.Nodes.Length];

    private PriorityQueue<int, float> _queue = new();

    public int[] Dijkstras(int start, int target)
    {
        this._queue.Clear();
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
