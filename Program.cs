using System.Numerics;
using Raylib_cs;

namespace Scout;

internal class Program
{
    private static MapData? _mapData;
    private static MapViewer? _mapViewer;

    private static void Main()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1600, 900, "Scout");
        Raylib.SetTargetFPS(60);

        var loadTask = Task.Run(() =>
        {
            _mapData = new("Assets/luxembourg.osm.pbf");
            _mapData.Load();
            _mapViewer = new(_mapData);
        });

        while (!Raylib.WindowShouldClose())
        {
            if (loadTask.IsCompletedSuccessfully)
            {
                ViewerScreen();
            }
            else if (loadTask.IsFaulted)
            {
                ExceptionScreen(loadTask.Exception?.GetBaseException());
            }
            else if (_mapViewer == null)
            {
                LoadingScreen();
            }
        }

        Raylib.CloseWindow();
    }

    private static void ViewerScreen() 
    {
        _mapViewer.Update();

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        _mapViewer.Draw();
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
