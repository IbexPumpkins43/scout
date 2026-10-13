using System.Numerics;

using Raylib_cs;

namespace Scout.Ui.Elements;

internal enum LabelButtonAlign
{
    Left,
    Middle,
    Right
}

internal class LabelButton : Container
{
    public Label Label;
    public LabelButtonAlign LabelAlign = LabelButtonAlign.Middle;
    public UiDesiredSizeMode DesiredSizeMode = UiDesiredSizeMode.FillParent;

    public event Action? OnClick;

    private bool _isPressed;

    public LabelButton(string text)
    {
        // Configure the label
        this.Label = new Label(text)
        {
            DesiredSizeMode = this.LabelAlign == LabelButtonAlign.Middle
                ? UiDesiredSizeMode.FillParent
                : UiDesiredSizeMode.FitToContent
        };

        // Configure the container
        this.Orientation = ContainerOrientation.Horizontal;
        this.Children = [this.Label];
        this.BackgroundColour = UiManager.ColourScheme.ButtonBackgroundColour;
    }

    public override void Initialise()
    {
        this.Label.DesiredSizeMode = this.LabelAlign == LabelButtonAlign.Middle
            ? UiDesiredSizeMode.FillParent
            : UiDesiredSizeMode.FitToContent;

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

    private void HandleOnMouseHover(Vector2 position, Vector2 delta)
    {
        this.UpdateAppearance(position);
    }

    private void HandleOnMouseDown(MouseButton button, Vector2 position, Vector2 delta)
    {
        if (this.Hidden || button != MouseButton.Left)
        {
            return;
        }

        this._isPressed = Raylib.CheckCollisionPointRec(position, this.Bounds);
        this.UpdateAppearance(position);
    }

    private void HandleOnMouseUp(MouseButton button, Vector2 position, Vector2 delta)
    {
        if (this.Hidden || button != MouseButton.Left)
        {
            return;
        }

        bool wasClicked = this._isPressed && Raylib.CheckCollisionPointRec(position, this.Bounds);

        this._isPressed = false;
        this.UpdateAppearance(position);

        if (wasClicked)
        {
            this.OnClick?.Invoke();
        }
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
