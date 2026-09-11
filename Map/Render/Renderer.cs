using System.Numerics;
using Raylib_cs;

namespace Scout.Map;

internal class Renderer(RenderData renderData, GraphData graphData)
{
    private RenderData _renderData = renderData;
    private GraphData _graphData = graphData;
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

    public void Draw(int[]? path = null) 
    {
        Raylib.BeginMode2D(this._camera);
        this.DrawRoads();
        this.DrawBuildings();
        Raylib.EndMode2D();

        this.DrawPlaces();
    }

    private void DrawRoads()
    {
        BoundBox viewBounds = this.GetViewBoundBox();

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

                    RoadStyle style = Road.GetRoadStyle(road.Type);

                    if (this._camera.Zoom < style.MinimumZoom)
                    {
                        continue;
                    }

                    int end = road.Start + road.Count - 1;
                    for (int index = road.Start; index < end; index++)
                    {
                        Raylib.DrawLineEx(
                            pointsPtr[index],
                            pointsPtr[index + 1],
                            style.Thickness / this._camera.Zoom, // MathF.Sqrt(this._camera.Zoom),
                            style.Colour);
                    }
                }
            }
        }
    }

    private void DrawPlaces()
    {
        foreach (DrawablePlace place in this._renderData.Places)
        {
            Vector2 worldPosition = new((float)place.Position.X, (float)place.Position.Y);
            Vector2 screenPosition = Raylib.GetWorldToScreen2D(worldPosition, this._camera);

            PlaceStyle style = Place.GetPlaceStyle(place.Type);

            if (this._camera.Zoom < style.MinimumZoom)
            {
                continue;
            }

            Raylib.DrawText(
                place.Name,
                (int)screenPosition.X,
                (int)screenPosition.Y,
                style.FontSize,
                Color.Black);
        }
    }

    private void DrawBuildings()
    {
        // TODO: implement this
    }

    private void DrawPath(int[]? path)
    {
        if (path == null)
        {
            return;
        }

        for (int index = 0; index < path.Length - 1; index++)
        {
            GraphNode firstNode =
                this._graphData.Nodes[path[index]];

            GraphNode secondNode =
                this._graphData.Nodes[path[index + 1]];

            Vector2 firstPosition = new(
                (float)firstNode.Position.X,
                (float)firstNode.Position.Y);

            Vector2 secondPosition = new(
                (float)secondNode.Position.X,
                (float)secondNode.Position.Y);

            Raylib.DrawLineV(firstPosition, secondPosition, Color.Red);
        }
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
