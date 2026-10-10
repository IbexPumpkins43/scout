using Raylib_cs;

namespace Scout.Ui.Elements;

internal class LabelButton : Container
{
    public Label Label;

    public LabelButton(string text)
    {
        // Configure the label
        this.Label = new Label(text)
        {
            DesiredSizeMode = UiDesiredSizeMode.FillParent
        };

        // Configure the container
        this.Orientation = ContainerOrientation.Horizontal;
        this.DesiredSize = new(
            UiLength.Flexible,
            this.Label.FontSize + Style.Padding.InnerPx * 2);
        this.Children = [this.Label];
        this.IsEventDeadEnd = true;
    }

    public override void Render()
    {
        // Button background + border
        Raylib.DrawRectangleRec(this.Bounds, UiManager.ColourScheme.ButtonBackgroundColour);
        Raylib.DrawRectangleLinesEx(this.Bounds, 1.0f, UiManager.ColourScheme.BorderColour);
    }
}
