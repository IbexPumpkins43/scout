using Raylib_cs;

namespace Scout.Ui.Elements;

internal class LabelButton : Container
{
    public string Text;

    public LabelButton(string text)
    {
        this.Text = text;

        // Configure the container
        this.Orientation = ContainerOrientation.Horizontal;
        this.Fill = ContainerFill.FillBoth;
        this.Children =
        [
            new Label()
            {
                Text = this.Text,
                DesiredSizeMode = UiDesiredSizeMode.FillParent,
            }
        ];
    }

    public override void UpdateLayout()
    {
    }

    public override void Render()
    {
        Raylib.DrawRectangleRec(this.Bounds, UiManager.ColourScheme.ButtonBackgroundColour);
        Raylib.DrawRectangleLinesEx(this.Bounds, 4.0f, UiManager.ColourScheme.BorderColour);
    }
}
