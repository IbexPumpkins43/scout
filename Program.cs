using System.Numerics;
using Raylib_cs;

namespace Scout;

internal class Program
{
    private static RenderData? _renderData;
    private static Renderer? _renderer;
    private static GraphData? _graphData;
    private static Graph? _graph;
    private static int[]? _path;

    private static void Main()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1600, 900, "Scout");
        Raylib.SetTargetFPS(60);

        Task<MapData> loadTask = Task.Run(() =>
        {
            MapImporter mapImporter = new(path: "Assets/luxembourg.osm.pbf");
            MapData mapData = mapImporter.Import();

            return mapData;
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
            else if (_renderer == null || _graph == null || _path == null)
            {
                _renderData ??= loadTask.Result.RenderData;
                _renderer ??= new(renderData: _renderData);
                _graphData ??= loadTask.Result.GraphData;
                _graph ??= new(graphData: _graphData);
                
                _path = _graph.Dijkstras(new Random().Next(1000), new Random().Next(1000));
            }
            else
            {
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
        const int ringSize = 40;

        float time = (float)Raylib.GetTime();
        float angle = time * 180.0f;

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        Raylib.DrawRing(
            new(Raylib.GetScreenWidth() / 2 - ringSize, Raylib.GetScreenHeight() / 2 - ringSize),
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
