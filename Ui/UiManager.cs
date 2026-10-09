using Raylib_cs;
using Scout.Ui.Assets;
using Scout.Ui.Screen;

namespace Scout.Ui;

internal static class UiManager
{
    public static AssetsManager AssetsManager { get; private set; } = new();
    public static ScreenManager ScreenManager { get; private set; } = new();

    public static UiTree? Tree;

    public static void Cleanup()
    {
        AssetsManager.Dispose();
        ScreenManager.Dispose();
    }
}
