using System.Numerics;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    public void Label(string text)
    {
        Raylib.BeginTextureMode(this._baseTexture);
        Raylib.DrawTextEx(
            this.RegularFont,
            text,
            new(this._xOffset, this._yOffset),
            Style.RegularFontSize,
            Style.RegularFontSpacing,
            Style.TextColour);
        Raylib.EndTextureMode();

        this.UpdateOffsets(Raylib.MeasureText(text, Style.RegularFontSize), Style.RegularFontSize);
    }

    public void Icon(string icon)
    {
        Rectangle source = new(
            this._iconsLookup[icon],
            new(Style.IconSize, Style.IconSize));

        Vector2 position = new(this._xOffset, this._yOffset);

        Raylib.DrawTextureRec(this._icons, source, position, Color.White);
    }

    private void DrawBox(Rectangle button, Color borderColour, Color bgColour)
    {
        Raylib.DrawRectangleRec(button, bgColour);
        Raylib.DrawRectangleLines(
            (int)button.X,
            (int)button.Y,
            (int)button.Width,
            (int)button.Height,
            borderColour);
    }

    private void Tooltip(string tip)
    {
        int width = (int)Raylib.MeasureTextEx(this.RegularFont, tip, Style.SmallFontSize, Style.SmallFontSpacing).X + Style.InnerPadding * 2;
        int height = Style.SmallFontSize + Style.InnerPadding * 2;

        Vector2 mouse = Raylib.GetMousePosition();

        Raylib.BeginTextureMode(this._tooltipTexture);

        Raylib.DrawRectangle((int)mouse.X, (int)mouse.Y, width, height, Style.TooltipBgColour);
        Raylib.DrawRectangleLines((int)mouse.X, (int)mouse.Y, width, height, Style.BorderColour);

        Raylib.DrawTextEx(
            this.RegularFont,
            tip,
            new (mouse.X + Style.InnerPadding, mouse.Y + Style.InnerPadding),
            Style.SmallFontSize,
            Style.SmallFontSpacing,
            Style.TextColour);

        Raylib.EndTextureMode();
    }

    private void UpdateOffsets(int width, int height)
    {
        if (this.SameLine)
        {
            this._yOffset = Style.OuterPadding;
            this._xOffset += width + Style.OuterPadding;
        }
        else
        {
            this._xOffset = Style.OuterPadding;
            this._yOffset += height + Style.OuterPadding;
        }
    }
}
