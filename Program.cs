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
            MapData mapData = new(path: "Assets/luxembourg.osm.pbf");
            mapData.Load();

            RenderData renderData = new();
            renderData.Build(mapData);

            return renderData;
        });

        while (!Raylib.WindowShouldClose())
        {
            if (loadTask.IsFaulted)
            {
                ExceptionScreen(loadTask.Exception!.GetBaseException());
            }
            else if (!loadTask.IsCompletedSuccessfully)
            {
                LoadingScreen();
            }
            else
            {
                _renderer ??= new(renderData: loadTask.Result);
                ViewerScreen();
            }
        }

        Raylib.CloseWindow();
    }

    private static void ViewerScreen() 
    {
        _renderer!.Update();

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        _renderer.Draw();
        Raylib.EndDrawing();
    }

    private static void LoadingScreen() 
    {
        const int fontSize = 20;
        const int ringSize = 10;

        float time = (float)Raylib.GetTime();
        float angle = time * 180.0f;

        string loadingMessage = "Loading...";
        int loadingMessageWidth = Raylib.MeasureText(loadingMessage, fontSize) + 15;

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        Raylib.DrawText(loadingMessage, 0, 0, fontSize, Color.Black);
        Raylib.DrawRing(
            new(loadingMessageWidth, ringSize),
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
