using System.Numerics;

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

    public override void Initialise()
    {
        // Subscribe to events
        this.Root.MouseMoved += this.HandleMouseHover;
        this.Root.MousePressed += this.HandleMouseClick;
    }

    public override void Render()
    {
        base.Render();
        // Button border
        Raylib.DrawRectangleLinesEx(this.Bounds, 1.0f, UiManager.ColourScheme.BorderColour);
    }

    private void HandleMouseHover(Vector2 position)
    {
        this.BackgroundColour = Raylib.CheckCollisionPointRec(position, this.Bounds)
            ? UiManager.ColourScheme.ButtonBackgroundAltColour
            : UiManager.ColourScheme.ButtonBackgroundColour;
    }

    private void HandleMouseClick(MouseButton button, Vector2 position)
    {
    }
}
