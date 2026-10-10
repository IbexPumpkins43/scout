using System.Numerics;
using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Label : UiNode
{
    public required string Text;
    public Color Colour = UiManager.ColourScheme.TextColour;
    public Font Font = UiManager.AssetsManager.GetRegularFont(Style.Font.LargeSize);
    public int FontSize = Style.Font.MediumSize;
    public float FontSpacing = Style.Font.MediumSpacing;
    public UiDesiredSizeMode DesiredSizeMode = UiDesiredSizeMode.FitToContent;

    private Vector2 _textSize;
    private Vector2 _textPosition;

    public override void Measure()
    {
        this._textSize = Raylib.MeasureTextEx(
            this.Font,
            this.Text,
            this.FontSize,
            this.FontSpacing);

        this.DesiredSize = this.DesiredSizeMode switch
        {
            UiDesiredSizeMode.FitToContent => new(this._textSize.X, this._textSize.Y),
            UiDesiredSizeMode.FillParent => UiSize.Flexible
        };
    }

    public override void UpdateLayout()
    {
        this._textPosition = new(
            this.Bounds.X + (this.Bounds.Width / 2) - (this._textSize.X / 2),
            this.Bounds.Y + (this.Bounds.Height / 2) - (this._textSize.Y / 2));
    }

    public override void Render()
    {
        Raylib.DrawTextEx(
            this.Font,
            this.Text,
            this._textPosition,
            this.FontSize,
            this.FontSpacing,
            this.Colour);
    }
}
