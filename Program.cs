using Raylib_cs;
using Scout.Settings;
using Scout.Ui;
using Scout.Ui.Elements;
using Scout.Ui.Screen.Screens;

namespace Scout;

internal class Program
{
    private static void Main()
    {
        SettingsData.Load();

        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint);
        Raylib.InitWindow(1600, 900, "Scout");
        Raylib.SetTargetFPS(60);

        UiManager.ScreenManager.RegisterScreen<TestScreen>();
        UiManager.ScreenManager.SwitchToScreen<TestScreen>();

        while (!Raylib.WindowShouldClose())
        {
            if (UiManager.Tree != null)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.B))
                {
                    ((Container)UiManager.Tree.Children[0]).Hidden ^= true;
                }
                UiManager.Tree.Update();
                UiManager.Tree.Render();
            }
        }

        UiManager.Cleanup();
        Raylib.CloseWindow();
    }
}
