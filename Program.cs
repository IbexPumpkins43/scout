using Raylib_cs;
using Scout.Scenes;

namespace Scout;

internal class Program
{
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
