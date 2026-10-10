using System.Numerics;

using Raylib_cs;

namespace Scout.Ui.Elements;

internal class LabelButton : Container
{
    public Label Label;
    public UiDesiredSizeMode DesiredSizeMode = UiDesiredSizeMode.FillParent;
    public bool HideBorder;

    public event Action? OnClick;

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
        this.Children = [this.Label];
        this.IsEventDeadEnd = true;
        this.BackgroundColour = UiManager.ColourScheme.ButtonBackgroundColour;
    }

    public override void Initialise()
    {
        // Subscribe to events
        this.Root.OnMouseMove += this.HandleOnMouseHover;
        this.Root.OnMouseDown += this.HandleOnMouseDown;
        this.Root.OnMouseUp += this.HandleOnMouseUp;
    }

    public override void Measure()
    {
        Vector2 textSize = this.Label.MeasureText;

        this.DesiredSize = this.DesiredSizeMode switch
        {
            UiDesiredSizeMode.FitToContent => new(
                textSize.X + Style.Padding.InnerPx * 2,
                textSize.Y + Style.Padding.InnerPx * 2),
            UiDesiredSizeMode.FillParent => UiSize.Flexible
        };
    }

    public override void Render()
    {
        base.Render();

        // Button border
        if (!this.HideBorder)
        {
            Raylib.DrawRectangleLinesEx(this.Bounds, 1.0f, UiManager.ColourScheme.BorderColour);
        }
    }

    private void HandleOnMouseHover(Vector2 position, Vector2 delta)
    {
        this.UpdateAppearance(position);
    }

    private void HandleOnMouseDown(MouseButton button, Vector2 position, Vector2 delta)
    {
        if (button != MouseButton.Left)
        {
            return;
        }

        this._isPressed = Raylib.CheckCollisionPointRec(position, this.Bounds);
        this.UpdateAppearance(position);
    }

    private void HandleOnMouseUp(MouseButton button, Vector2 position, Vector2 delta)
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
