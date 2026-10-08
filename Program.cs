using Raylib_cs;
using Scout.Scenes;
using Scout.Settings;

namespace Scout;

internal class Program
{
    private static void Main()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(1600, 900, "Scout");
        Raylib.SetTargetFPS(60);

        Settings.Settings.Load();

        using SceneManager sceneManager = new();
        //sceneManager.RegisterScene<LoadingScene>();
        sceneManager.RegisterScene<ErrorScene>();
        sceneManager.RegisterScene<WelcomeScene>();
        //sceneManager.RegisterScene<ViewerScene>();
        sceneManager.SwitchTo<WelcomeScene>();
        sceneManager.Run();

        Raylib.CloseWindow();
    }
}
