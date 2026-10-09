using Raylib_cs;
using Scout.Settings;
using Scout.Ui;
using Scout.Ui.Screens;

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
            UiManager.ScreenManager.RenderCurrentScreen();
        }

        Raylib.CloseWindow();
    }
}
