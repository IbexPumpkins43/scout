using System.Numerics;

using Raylib_cs;

namespace Scout.Ui.Elements;

internal class LabelButton : Container
{
    public Label Label;
    public event Action? Clicked;

    private bool _isPressed;

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
        this.Root.MouseDown += this.HandleMouseDown;
        this.Root.MouseUp += this.HandleMouseUp;
    }

    public override void Render()
    {
        base.Render();

        // Button border
        Raylib.DrawRectangleLinesEx(this.Bounds, 1.0f, UiManager.ColourScheme.BorderColour);
    }

    private void HandleMouseHover(Vector2 position)
    {
        this.UpdateAppearance(position);
    }

    private void HandleMouseDown(MouseButton button, Vector2 position)
    {
        if (button != MouseButton.Left)
        {
            return;
        }

        this._isPressed = Raylib.CheckCollisionPointRec(position, this.Bounds);
        this.UpdateAppearance(position);
    }

    private void HandleMouseUp(MouseButton button, Vector2 position)
    {
        if (button != MouseButton.Left)
        {
            return;
        }

        this._isPressed = false;
        this.UpdateAppearance(position);
    }

    private void UpdateAppearance(Vector2 mousePosition)
    {
        bool isHovered = Raylib.CheckCollisionPointRec(mousePosition, this.Bounds);
        bool isPressed = this._isPressed && isHovered;

        this.BackgroundColour = isPressed
            ? UiManager.ColourScheme.ButtonDownBackgroundColour
            : isHovered
                ? UiManager.ColourScheme.ButtonBackgroundAltColour
                : UiManager.ColourScheme.ButtonBackgroundColour;

        this.Label.Colour = isPressed
            ? UiManager.ColourScheme.ButtonDownTextColour
            : UiManager.ColourScheme.TextColour;
    }
}
