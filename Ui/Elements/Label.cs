using System.Numerics;
using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Label : UiNode
{
    private string _text;
    public string Text
    {
        get => this._text;
        set => this.SetProperty(ref this._text, value);
    }

    private Color _colour = UiManager.ColourScheme.TextColour;
    public Color Colour
    {
        get => this._colour;
        set => this.SetProperty(ref this._colour, value);
    }

    private Font _font = UiManager.AssetsManager.GetRegularFont(Style.Font.MediumSize);
    private int _fontSize = Style.Font.MediumSize;
    public int FontSize
    {
        get => this._fontSize;
        set
        {
            if (this._fontSize == value)
            {
                return;
            }

            this._font = UiManager.AssetsManager.GetRegularFont(value);
            this._fontSize = value;
            this.InvalidateLayout();
        }
    }

    private float _fontSpacing = Style.Font.MediumSpacing;
    public float FontSpacing
    {
        get => this._fontSpacing;
        set => this.SetProperty(ref this._fontSpacing, value);
    }

    private UiDesiredSizeMode _desiredSizeMode = UiDesiredSizeMode.FitToContent;
    public UiDesiredSizeMode DesiredSizeMode
    {
        get => this._desiredSizeMode;
        set => this.SetProperty(ref this._desiredSizeMode, value);
    }

    private Vector2 _textSize;
    private Vector2 _textPosition;


    public Label(string text)
    {
        this.Text = text;
    }

    public Vector2 MeasureText => Raylib.MeasureTextEx(
        this._font,
        this.Text,
        this.FontSize,
        this.FontSpacing);

    public override void Measure()
    {
        this._textSize = this.MeasureText;

        this.DesiredSize = this.DesiredSizeMode switch
        {
            UiDesiredSizeMode.FitToContent => new(this._textSize.X, this._textSize.Y),
            UiDesiredSizeMode.FillParent => UiSize.Flexible
        };
    }

    public override void UpdateLayout()
    {
        // Recentre the text
        this._textPosition = new(
            this.Bounds.X + (this.Bounds.Width / 2) - (this._textSize.X / 2),
            this.Bounds.Y + (this.Bounds.Height / 2) - (this._textSize.Y / 2));
    }

    public override void Render()
    {
        Raylib.DrawTextEx(
            this._font,
            this.Text,
            this._textPosition,
            this.FontSize,
            this.FontSpacing,
            this.Colour);
    }
}
