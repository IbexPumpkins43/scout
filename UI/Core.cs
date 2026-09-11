using System.Numerics;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    public void Label(string text)
    {
        Raylib.BeginTextureMode(this._baseTexture);
        Raylib.DrawText(text, _xOffset, _yOffset, Style.FontSize, Style.TextColour);
        Raylib.EndTextureMode();

        this.UpdateOffsets(Raylib.MeasureText(text, Style.FontSize), Style.FontSize);
    }

    public void Icon(string icon)
    {
        Rectangle source = new(
            this._iconsLookup[icon],
            new(Style.IconSize, Style.IconSize));

        Vector2 position = new(_xOffset + Style.OuterPadding, _yOffset + Style.OuterPadding);

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
        int width = Raylib.MeasureText(tip, Style.SmallFontSize) + Style.InnerPadding * 2;
        int height = Style.SmallFontSize + Style.InnerPadding * 2;

        Vector2 mouse = Raylib.GetMousePosition();

        Raylib.BeginTextureMode(this._tooltipTexture);
        
        Raylib.DrawRectangle((int)mouse.X, (int)mouse.Y, width, height, Style.TooltipBgColour);
        Raylib.DrawRectangleLines((int)mouse.X, (int)mouse.Y, width, height, Style.BorderColour);
        
        Raylib.DrawText(
            tip, 
            (int)mouse.X + Style.InnerPadding, 
            (int)mouse.Y + Style.InnerPadding, 
            Style.SmallFontSize, 
            Style.TextColour);
        
        Raylib.EndTextureMode();
    }

    private void UpdateOffsets(int width, int height)
    {
        if (SameLine)
        {
            this._xOffset += width + Style.OuterPadding;
        }
        else
        {
            this._yOffset += height + Style.OuterPadding;
        }
    }
}