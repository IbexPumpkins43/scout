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

        if (SameLine)
        {
            _xOffset += Raylib.MeasureText(text, Style.FontSize);
        }
        else
        {
            _yOffset += Style.FontSize;
        }
    }

    public bool LabelButton(string text, string? tooltip = null)
    {
        return this.Button(
            text: text,
            width: Raylib.MeasureText(text, Style.FontSize),
            height: Style.FontSize,
            tooltip: tooltip,
            isIcon: false);
    }

    public bool IconButton(string icon, string? tooltip = null)
    {        
        return this.Button(
            text: icon,
            width: Style.IconSize,
            height: Style.IconSize,
            tooltip: tooltip,
            isIcon: true);
   }

    private bool Button(
        string text, 
        int width,
        int height, 
        string? tooltip = null, 
        bool isIcon = false)
    {
        width += Style.InnerPadding * 2;
        height += Style.InnerPadding * 2;

        Color borderColour = Style.BorderColour;
        Color bgColour = Style.ButtonBgColour;
        bool returnValue = false;
        bool drawTooltip = false;

        Rectangle button = new Rectangle(
            x: _xOffset + Style.OuterPadding, 
            y: _yOffset + Style.OuterPadding, 
            width: width, 
            height: height);
        Vector2 mouse = Raylib.GetMousePosition();
        if (Raylib.CheckCollisionPointRec(mouse, button))
        {
            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                returnValue = true;
            }
            else if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                borderColour = Style.BorderAltColour;
                bgColour = Style.ButtonBgAltColour;
            }
            else if (tooltip != null)
            {
                drawTooltip = true;
            }
        }


        Raylib.BeginTextureMode(this._baseTexture);
        Raylib.DrawRectangle(
            _xOffset + Style.OuterPadding, 
            _yOffset + Style.OuterPadding, 
            width, 
            height, 
            bgColour);
        Raylib.DrawRectangleLines(
            _xOffset + Style.OuterPadding, 
            _yOffset + Style.OuterPadding, 
            width, 
            height, 
            borderColour);
        if (isIcon)
        {
            Vector2 iconLocation = this._iconsLookup[text];
            Raylib.DrawTextureRec(
                this._icons, 
                new(
                    position: iconLocation, 
                    width: Style.IconSize, 
                    height: Style.IconSize), 
                new(
                    _xOffset + Style.InnerPadding + Style.OuterPadding, 
                    _yOffset + Style.InnerPadding + Style.OuterPadding),
                Color.White);
        }
        else
        {
            Raylib.DrawText(
                text, 
                _xOffset + Style.InnerPadding + Style.OuterPadding, 
                _yOffset + Style.InnerPadding + Style.OuterPadding, 
                Style.FontSize, 
                Style.TextColour);
        }
        Raylib.EndTextureMode();

        if (drawTooltip)
        {
            this.Tooltip(tooltip);
        }

        if (this.SameLine)
        {
            _xOffset += width + Style.OuterPadding;
        }
        else
        {
            _yOffset += height + Style.OuterPadding;
        }

        return returnValue;
    }

    private void Tooltip(string tip)
    {
        int width = Raylib.MeasureText(tip, Style.SmallFontSize) + Style.InnerPadding * 2;
        int height = Style.SmallFontSize + Style.InnerPadding * 2;

        Vector2 mouse = Raylib.GetMousePosition();

        Raylib.BeginTextureMode(this._tooltipTexture);
        Raylib.DrawRectangle((int)mouse.X, (int)mouse.Y, width, height, Style.TooltipBgColour);
        Raylib.DrawRectangleLines((int)mouse.X, (int)mouse.Y, width, height, Style.BorderColour);
        Raylib.DrawText(tip, (int)mouse.X + Style.InnerPadding, (int)mouse.Y + Style.InnerPadding, Style.SmallFontSize, Style.TextColour);
        Raylib.EndTextureMode();
    }

}