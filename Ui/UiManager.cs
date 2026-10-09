using Raylib_cs;
using Scout.Ui.Assets;
using Scout.Ui.Screen;
using Scout.Ui.Style.Colour;

namespace Scout.Ui;

internal static class UiManager
{
    public static AssetsManager AssetsManager { get; private set; } = new();
    public static ScreenManager ScreenManager { get; private set; } = new();
    public static IColourScheme ColourScheme { get; private set; } = new LightMode();

    public static UiTree? Tree;

    public static void Cleanup()
    {
        AssetsManager.Dispose();
        ScreenManager.Dispose();
    }
}
