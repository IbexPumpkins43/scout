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

        SceneManager sceneManager = new();
        sceneManager.RegisterScene<LoadingScene>();
        sceneManager.RegisterScene<ErrorScene>();
        sceneManager.RegisterScene<ViewerScene>();
        sceneManager.SwitchTo<LoadingScene>();
        sceneManager.Run();

        Raylib.CloseWindow();
    }
}
