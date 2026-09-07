using System.Numerics;
using System.Runtime.InteropServices;
using Raylib_cs;

namespace Scout;

internal readonly record struct BoundBox(
    float MinX,
    float MaxX,
    float MinY,
    float MaxY);

internal record DrawableRoad(
    List<Vector2> Points,
    BoundBox Bounds);

internal class MapViewer
{
    private MapData _mapData;
    private List<DrawableRoad> _drawableRoads;
    private Camera2D _camera;

    public MapViewer(MapData mapData) 
    {
        this._mapData = mapData;

        this.PrepareCamera();
        this.PrepareRoads();
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
        Raylib.DrawText($"{this._mapData.Path}", 0, 20, 20, Color.Black);

        Raylib.BeginMode2D(this._camera);
        Raylib.DrawEllipse(0, 0, 10.0f, 10.0f, Color.Red);
        this.DrawRoads();
        this.DrawBuildings();
        Raylib.EndMode2D();
    }

    private void DrawRoads()
    {
        foreach (var road in this._drawableRoads)
        {
            // TODO: only draw roads in the visible view
            unsafe
            {
                var pointsSpan = CollectionsMarshal.AsSpan(road.Points);
                fixed (Vector2* pointsPtr = pointsSpan)
                {
                    Raylib.DrawLineStrip(pointsPtr, road.Points.Count, Color.Green);
                }
            }
        }
    }

    private void DrawBuildings()
    {
        // TODO: implement this
    }

    private void PrepareCamera()
    {
        this._camera = new Camera2D(
            offset: new Vector2(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
            target: new Vector2(0.0f, 0.0f),
            rotation: 0.0f,
            zoom: 1.0f);
    }

    private void PrepareRoads()
    {
        this._drawableRoads = new List<DrawableRoad>();

        // Precompute road geometry and bounds so off-screen roads can be culled cheaply
        foreach (var (road, roadIndex) in this._mapData.Roads.Select((road, index) => (road, index)))
        {
            var points = new List<Vector2>();
            foreach (var nodeId in road.NodeIDs)
            {
                if (this._mapData.NodePositions.ContainsKey(nodeId))
                {
                    var node = this._mapData.NodePositions[nodeId];
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
    }
}
