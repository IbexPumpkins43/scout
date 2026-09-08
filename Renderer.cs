using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Scout;

internal readonly record struct BoundBox(
    float MinX,
    float MaxX,
    float MinY,
    float MaxY);

internal readonly record struct DrawableRoad(
    int Start,
    int Count,
    BoundBox Bounds);

internal class RenderData
{
    public Vector2[] Points { get; private set; }
    public DrawableRoad[] Roads { get; private set; }

    public RenderData() 
    {
    }

    public void Build(MapData mapData) 
    {
        var allPoints = new List<Vector2>();
        var roads = new List<DrawableRoad>();

        foreach (var road in mapData.Roads)
        {       
            var start = allPoints.Count;

            var minX = float.MaxValue;
            var maxX = float.MinValue;
            var minY = float.MaxValue;
            var maxY = float.MinValue;

            foreach (var nodeId in road.NodeIDs)
            {
                if (!mapData.NodePositions.TryGetValue(nodeId, out var node))
                {
                    continue;
                }

                var point = new Vector2((float)node.Latitude, (float)node.Longitude);
                allPoints.Add(point);

                minX = MathF.Min(minX, point.X);
                maxX = MathF.Max(maxX, point.X);
                minY = MathF.Min(minY, point.Y);
                maxY = MathF.Max(maxY, point.Y);
            }

            var count = allPoints.Count - start;
            if (count >= 2)
            {
                var bounds = new BoundBox(
                    MaxX: maxX,
                    MinX: minX,
                    MaxY: maxY,
                    MinY: minY);
                var drawableRoad = new DrawableRoad(
                    Start: start,
                    Count: count,
                    Bounds: bounds);
                roads.Add(drawableRoad);
            }
            else
            {
                // Discard an invalid road
                allPoints.RemoveRange(start, count);
            }
        }

        // TODO :  A big memory spike occurs here as both the list and the array exist at the 
        //         same time
        this.Points = allPoints.ToArray();
        this.Roads = roads.ToArray();
    }
}

internal class Renderer
{
    private RenderData _renderData;
    private Camera2D _camera;

    public Renderer(RenderData renderData) 
    {
        this._renderData = renderData;
        this._camera = new Camera2D(
            offset: new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
            target: new Vector2(0.0f, 0.0f),
            rotation: 0.0f,
            zoom: 1.0f);
    }

    public void Update() 
    {
        if (Raylib.IsWindowResized())
        {
            this._camera.Offset.X = Raylib.GetScreenWidth() / 2;
            this._camera.Offset.Y = Raylib.GetScreenHeight() / 2;
        }

        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            var mouseDelta = Raylib.GetMouseDelta();
            this._camera.Target.X -= mouseDelta.X / this._camera.Zoom;
            this._camera.Target.Y -= mouseDelta.Y / this._camera.Zoom;
        }

        this._camera.Zoom *= MathF.Pow(1.1f, Raylib.GetMouseWheelMove());
        this._camera.Zoom = Raymath.Clamp(this._camera.Zoom, 0.001f, 10.0f);
    }

    public void Draw() 
    {
        Raylib.DrawFPS(0, 0);

        Raylib.BeginMode2D(this._camera);
        Raylib.DrawEllipse(0, 0, 10.0f, 10.0f, Color.Red);
        this.DrawRoads();
        this.DrawBuildings();
        Raylib.EndMode2D();
    }

    private void DrawRoads()
    {
        // TODO: only draw roads in the visible view
        unsafe
        {
            fixed (Vector2* pointsPtr = _renderData.Points)
            {
                foreach (var road in _renderData.Roads)
                {
                    Raylib.DrawLineStrip(pointsPtr + road.Start, (int)road.Count, Color.Green);
                }
            }
        }
    }

    private void DrawBuildings()
    {
        // TODO: implement this
    }

    /*private void PrepareRoads(MapData mapData)
    {
        this._drawableRoads = new List<DrawableRoad>();

        // Precompute road geometry and bounds so off-screen roads can be culled cheaply
        foreach (var (road, roadIndex) in mapData.Roads.Select((road, index) => (road, index)))
        {
            var points = new List<Vector2>();
            foreach (var nodeId in road.NodeIDs)
            {
                if (mapData.NodePositions.ContainsKey(nodeId))
                {
                    var node = mapData.NodePositions[nodeId];
                    points.Add(new Vector2((float)node.Latitude, (float)node.Longitude));
                }
            }

            if (points.Count >= 2)
            {
                var bounds = new BoundBox(
                    MinX: points.Min(point => point.X),
                    MaxX: points.Max(point => point.X),
                    MinY: points.Min(point => point.Y),
                    MaxY: points.Max(point => point.Y));

                var drawableRoad = new DrawableRoad(
                    Points: points,
                    Bounds: bounds);
                
                this._drawableRoads.Add(drawableRoad);
            }
        }
    }*/
}
