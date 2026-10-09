using System.Numerics;
using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Label : UiNode
{
    public string Text;
    public Color Colour = UiManager.ColourScheme.TextColour;
    public Font Font = UiManager.AssetsManager.RegularFont;
    public int FontSize = Style.Font.RegularSize;
    public float FontSpacing = Style.Font.RegularSpacing;

    private Vector2 _textSize;
    private Vector2 _textPosition;

    public override void UpdateLayout()
    {
        this._textSize = Raylib.MeasureTextEx(
            UiManager.AssetsManager.RegularFont,
            this.Text,
            this.FontSize,
            this.FontSpacing);
        this._textPosition = new(
            this.Bounds.X + (this.Bounds.Width / 2) - (this._textSize.X / 2),
            this.Bounds.Y + (this.Bounds.Height / 2) - (this._textSize.Y / 2));
        this.DesiredSize = new UiSize(this._textSize.X, this._textSize.Y);
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
