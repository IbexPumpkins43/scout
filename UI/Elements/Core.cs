using System.Numerics;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    public void Label(
        string text,
        Color? colour = null,
        float? x = null,
        float? y = null)
    {
        Raylib.BeginTextureMode(this._baseTexture);
        Raylib.DrawTextEx(
            this.RegularFont,
            text,
            new(x ?? this._xOffset, y ?? this._yOffset),
            Style.RegularFontSize,
            Style.RegularFontSpacing,
            colour ?? Style.TextColour);
        Raylib.EndTextureMode();

        this.UpdateOffsets(Raylib.MeasureText(text, Style.RegularFontSize), Style.RegularFontSize);
    }

    public void Icon(string icon, float? x = null, float? y = null)
    {
        Rectangle source = new(
            position: this._iconsLookup[icon],
            size: new(Style.IconSize, Style.IconSize));

        Vector2 position = new(x ?? this._xOffset, y ?? this._yOffset);

        Raylib.DrawTextureRec(this._icons, source, position, Color.White);

        if (x != null && y != null)
        {
            this.UpdateOffsets(source.Width, source.Height);
        }
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
        int width = (int)Raylib.MeasureTextEx(
            this.RegularFont,
            tip,
            Style.SmallFontSize,
            Style.SmallFontSpacing).X + Style.InnerPadding * 2;
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
            this._xOffset += width + Style.OuterPadding;
        }
        else
        {
            this._yOffset += height + Style.OuterPadding;
        }
    }

    private void UpdateOffsets(float width, float height) =>
        this.UpdateOffsets((int)width, (int)height);
}
