using System.Numerics;
using Raylib_cs;

namespace Scout;

internal class Program
{
    private static Renderer? _renderer;

    private static void Main()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1600, 900, "Scout");
        Raylib.SetTargetFPS(60);

        Task<RenderData> loadTask = Task.Run(() =>
        {
            var mapData = new MapData("Assets/luxembourg.osm.pbf");
            mapData.Load();

            var renderData = new RenderData();
            renderData.Build(mapData);

            return renderData;
        });

        while (!Raylib.WindowShouldClose())
        {
            if (loadTask.IsCompletedSuccessfully && _renderer == null)
            {
                _renderer = new(loadTask.Result);
            }
            if (loadTask.IsCompletedSuccessfully)
            {
                ViewerScreen();
            }
            else if (loadTask.IsFaulted)
            {
                ExceptionScreen(loadTask.Exception?.GetBaseException());
            }
            else if (_renderer == null)
            {
                LoadingScreen();
            }
        }

        Raylib.CloseWindow();
    }

    private static void ViewerScreen() 
    {
        _renderer.Update();

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        _renderer.Draw();
        Raylib.EndDrawing();
    }

    private static void LoadingScreen() 
    {
        const int fontSize = 20;
        const int ringSize = 10;

        var time = (float)Raylib.GetTime();
        var angle = time * 180.0f;
        var loadingMessage = "Loading...";
        var loadingMessageWidth = Raylib.MeasureText(loadingMessage, fontSize) + 15;

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        Raylib.DrawText(loadingMessage, 0, 0, fontSize, Color.Black);
        Raylib.DrawRing(
            new Vector2(loadingMessageWidth, ringSize),
            ringSize / 2,
            ringSize,
            angle,
            angle + 240,
            32,
            Color.SkyBlue
        );
        Raylib.EndDrawing();
    }

    private static void ExceptionScreen(Exception exception) 
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        Raylib.DrawText($"{exception.Message}", 0, 0, 20, Color.Red);
        Raylib.EndDrawing();
    }
}
