using Raylib_cs;

namespace Scout.Ui.Elements;

internal readonly record struct Menu(
    string Name,
    string[] Entires);

internal class Menubar : Container
{
    public Menu[] Menus;

    public Menubar(Menu[] menus)
    {
        this.Menus = menus;

        // Container configuration
        this.Orientation = ContainerOrientation.Horizontal;
        this.Children =
        [
            new LabelButton("File"){ DesiredSizeMode = UiDesiredSizeMode.FitToContent, HideBorder = true },
            new LabelButton("Edit"){ DesiredSizeMode = UiDesiredSizeMode.FitToContent, HideBorder = true },
            new LabelButton("Help"){ DesiredSizeMode = UiDesiredSizeMode.FitToContent, HideBorder = true },
        ];
        this.IsEventDeadEnd = true;
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
