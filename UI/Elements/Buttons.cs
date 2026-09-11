using System.Numerics;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    public bool LabelButton(string text, string? tooltip = null)
    {
        Rectangle button = new(
            x: _xOffset + Style.OuterPadding,
            y: _yOffset + Style.OuterPadding,
            width: Raylib.MeasureText(text, Style.FontSize) + Style.InnerPadding * 2,
            height: Style.FontSize + Style.InnerPadding * 2);

        var (borderColour, bgColour, showTooltip, wasClicked) = this.UpdateButton(button);

        Raylib.BeginTextureMode(this._baseTexture);

        this.DrawBox(button, borderColour, bgColour);
        
        Raylib.DrawText(
            text, 
            _xOffset + Style.InnerPadding + Style.OuterPadding, 
            _yOffset + Style.InnerPadding + Style.OuterPadding, 
            Style.FontSize, 
            Style.TextColour);

        Raylib.EndTextureMode();       

        if (showTooltip && tooltip != null)
        {
            this.Tooltip(tooltip);
        }

        this.UpdateOffsets((int)button.Width, (int)button.Height);

        return wasClicked;
    }

    public bool IconButton(string icon, string? tooltip = null)
    {
        Rectangle button = new(
            x: _xOffset + Style.OuterPadding,
            y: _yOffset + Style.OuterPadding,
            width: Style.IconSize + Style.InnerPadding * 2,
            height: Style.IconSize + Style.InnerPadding * 2);

        var (borderColour, bgColour, showTooltip, wasClicked) = this.UpdateButton(button);

        Raylib.BeginTextureMode(this._baseTexture);

        this.DrawBox(button, borderColour, bgColour);
    
        Vector2 iconLocation = this._iconsLookup[icon];
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

        Raylib.EndTextureMode();       

        if (showTooltip && tooltip != null)
        {
            this.Tooltip(tooltip);
        }

        this.UpdateOffsets((int)button.Width, (int)button.Height);

        return wasClicked;
    }

    private (Color, Color, bool, bool) UpdateButton(Rectangle button)
    {
        Color borderColour = Style.BorderColour;
        Color bgColour = Style.ButtonBgColour;
        bool showTooltip = false;
        bool wasClicked = false;

        Vector2 mouse = Raylib.GetMousePosition();
        if (Raylib.CheckCollisionPointRec(mouse, button))
        {
            if (Raylib.IsMouseButtonReleased(MouseButton.Left))
            {
                wasClicked = true;
            }
            else if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                borderColour = Style.BorderAltColour;
                bgColour = Style.ButtonBgAltColour;
            }
            else
            {
                showTooltip = true;
            }
        }

        return (borderColour, bgColour, showTooltip, wasClicked);
    }
}