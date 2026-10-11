using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Menubar : Container
{
    public Menu[] Menus;

    public Menubar(Menu[] menus)
    {
        this.Menus = menus;

        // Container configuration
        this.Orientation = ContainerOrientation.Horizontal;
        this.Children = this.Menus;
        this.BackgroundColour = UiManager.ColourScheme.ButtonBackgroundColour;
    }

    public override void Measure()
    {
        this.DesiredSize = new(
            UiLength.Flexible,
            Style.Font.MediumSize + Style.Padding.InnerPx * 2 + 1.0f);
    }

    public override void Render()
    {
        base.Render();

        // Button border
        Raylib.DrawRectangleLinesEx(this.Bounds, 1.0f, UiManager.ColourScheme.BorderColour);
    }
}
