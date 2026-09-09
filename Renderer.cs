using System.Numerics;
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
    public Vector2[] Points { get; private set; } = Array.Empty<Vector2>();
    public DrawableRoad[] Roads { get; private set; } = Array.Empty<DrawableRoad>();

    public void Build(MapNodePositions nodePositions, MapRoads allRoads) 
    {
        List<Vector2> allPoints = new();
        List<DrawableRoad> roads = new();

        foreach (OSMWay road in allRoads)
        {       
            int start = allPoints.Count;

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (long nodeId in road.NodeIds)
            {
                if (!nodePositions.TryGetValue(nodeId, out MapPosition node))
                {
                    continue;
                }

                Vector2 point = new((float)node.X, (float)node.Y);
                allPoints.Add(point);

                minX = MathF.Min(minX, point.X);
                maxX = MathF.Max(maxX, point.X);
                minY = MathF.Min(minY, point.Y);
                maxY = MathF.Max(maxY, point.Y);
            }

            int count = allPoints.Count - start;
            if (count >= 2)
            {
                BoundBox bounds = new(
                    MinX: minX,
                    MaxX: maxX,
                    MinY: minY,
                    MaxY: maxY);
                DrawableRoad drawableRoad = new(
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

internal class Renderer(RenderData renderData)
{
    private RenderData _renderData = renderData;
    private Camera2D _camera = new(
        offset: new(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
        target: new(0.0f, 0.0f),
        rotation: 0.0f,
        zoom: 1.0f);

    public void Update() 
    {
        if (Raylib.IsWindowResized())
        {
            this._camera.Offset.X = Raylib.GetScreenWidth() / 2;
            this._camera.Offset.Y = Raylib.GetScreenHeight() / 2;
        }

        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            Vector2 mouseDelta = Raylib.GetMouseDelta();
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
        BoundBox viewBounds = this.GetViewBoundBox();

        // TODO: only draw roads in the visible view
        unsafe
        {
            fixed (Vector2* pointsPtr = this._renderData.Points)
            {
                foreach (DrawableRoad road in this._renderData.Roads)
                {
                    if (!this.BoundBoxIntersects(road.Bounds, viewBounds))
                    {
                        continue;
                    }

                    Raylib.DrawLineStrip(pointsPtr + road.Start, road.Count, Color.Green);
                }
            }
        }
    }

    private void DrawBuildings()
    {
        // TODO: implement this
    }

    private BoundBox GetViewBoundBox()
    {
        float halfWidth = Raylib.GetScreenWidth() / (2.0f * this._camera.Zoom);
        float halfHeight = Raylib.GetScreenHeight() / (2.0f * this._camera.Zoom);

        return new(
            MinX: this._camera.Target.X - halfWidth,
            MaxX: this._camera.Target.X + halfWidth,
            MinY: this._camera.Target.Y - halfHeight,
            MaxY: this._camera.Target.Y + halfHeight);
    }

    private bool BoundBoxIntersects(BoundBox first, BoundBox second)
    {
        return first.MaxX >= second.MinX
            && first.MinX <= second.MaxX
            && first.MaxY >= second.MinY
            && first.MinY <= second.MaxY;
    }
}
