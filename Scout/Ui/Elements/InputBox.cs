using Raylib_cs;

namespace Scout.Ui.Elements;

internal class InputBox : Container
{
    public string Input;
    public bool HideText;

    private Label _inputLabel;
    private bool _isFocused = false;

    public InputBox()
    {
        // Create the label to show the input
        this.Input = "";
        this._inputLabel = new(this.Input)
        {
            DesiredSizeMode = UiDesiredSizeMode.FitToContent,
        };

        // Configure the container
        this.BackgroundColour = UiManager.ColourScheme.Background;
        this.Orientation = ContainerOrientation.Horizontal;
        this.Border = true;
        this.Children =
        [
            this._inputLabel
        ];
    }

    public override void Initialise()
    {
        this.Root.OnKeyPress += (key) =>
        {
            // Remove char from end of string on backspace is there is anything to remove
            if (this._isFocused && this.Input.Length != 0 && key == KeyboardKey.Backspace)
            {
                this.Input = this.Input[..^1];
                this._inputLabel.Text = this.HideText
                    ? new string('*', this.Input.Length)
                    : this.Input;
            }
        };
        this.Root.OnCharPress += (keyChar) =>
        {
            // Add char to the string
            if (this._isFocused)
            {
                this.Input += keyChar;
                this._inputLabel.Text = this.HideText
                    ? new string('*', this.Input.Length)
                    : this.Input;
            }
        };
        this.Root.OnMouseDown += (button, position, delta) =>
        {
            if (button != MouseButton.Left)
            {
                return;
            }

            if (Raylib.CheckCollisionPointRec(position, this.Bounds))
            {
                this._isFocused = true;
            }
            else
            {
                this._isFocused = false;
            }
        };
    }

    public override void Measure()
    {
        // Ensures that the height of the box matches the text height
        // TODO : maybe refine this at some point
        this.DesiredSize = new(
            UiLength.Flexible,
            this._inputLabel.FontSize + Style.Padding.InnerPx * 2);
    }

    public override void UpdateLayout()
    {
    }

    public override void Render()
    {

        if (this._isFocused)
        {
            Raylib.DrawRectangleLinesEx(this.Bounds, 1.0f,
                UiManager.ColourScheme.ButtonDownBackgroundColour);
        }
        else
        {
            base.Render();
        }
    }
}
