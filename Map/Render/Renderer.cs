using System.Numerics;
using Raylib_cs;
using Scout.Map.Graph;
using Scout.Ui;
using Scout.Ui.Elements;

namespace Scout.Map;

internal class Renderer : IDisposable
{
    private RenderData _renderData;
    private GraphData _graphData;
    private Font _font;

    private bool _redraw = true;
    //private RenderTexture2D _texture = Raylib.LoadRenderTexture(
    //    Raylib.GetScreenWidth(),
    //    Raylib.GetScreenHeight());
    private Camera2D _camera = new(
        offset: new(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() / 2),
        target: new(0.0f, 0.0f),
        rotation: 0.0f,
        zoom: 1.0f);

    public Canvas Canvas { get; private set; }

    public Renderer(RenderData renderData, GraphData graphData, Font font)
    {
        this._renderData = renderData;
        this._graphData = graphData;
        this._font = font;

        this.Canvas = new();
        this.Canvas.OnResize += this.OnCanvasResize;
        this.Canvas.OnMouseDown += this.OnCanvasMouseDown;
        this.Canvas.OnMouseScroll += this.OnCanvasMouseScroll;
        this.Canvas.OnRerender += this.OnRerender;
    }

    public void Dispose()
    {
        this.Canvas.Dispose();
    }

    private void OnCanvasResize(float width, float height)
    {
        this._camera.Offset.X = width / 2;
        this._camera.Offset.Y = height / 2;
        this.Canvas.Invalidate();
    }

    private void OnCanvasMouseDown(MouseButton button, Vector2 position, Vector2 delta)
    {
        if (button == MouseButton.Left)
        {
            this._camera.Target.X -= delta.X / this._camera.Zoom;
            this._camera.Target.Y -= delta.Y / this._camera.Zoom;
            this.Canvas.Invalidate();
        }
    }

    private void OnCanvasMouseScroll(float wheel)
    {
        this._camera.Zoom *= MathF.Pow(1.1f, wheel);
        this._camera.Zoom = Raymath.Clamp(this._camera.Zoom, 0.001f, 10.0f);
        this.Canvas.Invalidate();
    }

    private void OnRerender()
    {
        Raylib.BeginMode2D(this._camera);
        Raylib.ClearBackground(new Color(250, 250, 250, 255));
        this.RenderRoads();
        this.RenderBuildings();
        Raylib.EndMode2D();

        this.RenderPlaces();
    }

    public void RenderPath(int[] path, bool moveCamera = false)
    {
        if (path.Length == 0)
        {
            return;
        }

        const float pathThickness = 4.0f;
        const float markerRadius = 8.0f;

        Raylib.BeginMode2D(this._camera);

        for (int index = 0; index < path.Length - 1; index++)
        {
            GraphNode firstNode = this._graphData.Nodes[path[index]];
            GraphNode secondNode = this._graphData.Nodes[path[index + 1]];

            Vector2 firstPosition = new((float)firstNode.Position.X, (float)firstNode.Position.Y);
            Vector2 secondPosition = new(
                (float)secondNode.Position.X,
                (float)secondNode.Position.Y);

            Raylib.DrawLineEx(firstPosition, secondPosition, pathThickness, Color.Pink);
        }

        GraphNode startNode = this._graphData.Nodes[path[0]];
        GraphNode endNode = this._graphData.Nodes[path[path.Length - 1]];

        Vector2 startPosition = new((float)startNode.Position.X, (float)startNode.Position.Y);
        Vector2 endPosition = new((float)endNode.Position.X, (float)endNode.Position.Y);

        Raylib.DrawCircleV(startPosition, markerRadius, Color.Blue);
        Raylib.DrawCircleV(endPosition, markerRadius, Color.Red);

        Raylib.EndMode2D();

        if (moveCamera)
        {
            this._camera.Target = startPosition;
        }

    }

    private void RenderRoads()
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
                            style.Thickness / this._camera.Zoom,
                            style.Colour);
                    }
                }
            }
        }
    }

    private void RenderPlaces()
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

            Raylib.DrawTextEx(
                this._font,
                place.Name,
                screenPosition,
                style.FontSize,
                0.0f,
                Color.Black);
        }
    }

    private void RenderBuildings()
    {
        if (this._camera.Zoom < 0.09f)
        {
            return;
        }

        BoundBox viewBounds = this.GetViewBoundBox();

        unsafe
        {
            fixed (Vector2* pointsPtr = this._renderData.Points)
            {
                foreach (DrawableBuilding building in this._renderData.Buildings)
                {
                    if (!this.BoundBoxIntersects(building.Bounds, viewBounds))
                    {
                        continue;
                    }

                    int end = building.Start + building.Count - 1;

                    for (int index = building.Start; index < end; index++)
                    {
                        Raylib.DrawLineEx(
                            pointsPtr[index],
                            pointsPtr[index + 1],
                            1.0f / this._camera.Zoom,
                            Color.Gray);
                    }

                    Raylib.DrawLineEx(
                        pointsPtr[end],
                        pointsPtr[building.Start],
                        1.0f / this._camera.Zoom,
                        Color.Gray);
                }
            }
        }
    }

    private BoundBox GetViewBoundBox()
    {
        float halfWidth = this.Canvas.Bounds.Width / (2.0f * this._camera.Zoom);
        float halfHeight = this.Canvas.Bounds.Height / (2.0f * this._camera.Zoom);

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
