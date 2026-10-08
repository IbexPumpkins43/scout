using System.Numerics;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager
{
    public bool LabelButton(string text, string? tooltip = null)
    {
        Rectangle button = new(
            x: this._xOffset,
            y: this._yOffset,
            width: Raylib.MeasureTextEx(
                this.RegularFont,
                text,
                Style.RegularFontSize,
                Style.RegularFontSpacing).X + Style.InnerPadding * 2,
            height: Style.RegularFontSize + Style.InnerPadding * 2);

        (Color textColour, Color bgColour, Color borderColour, bool showTooltip, bool wasClicked) =
            this.UpdateButton(button);

        Raylib.BeginTextureMode(this._baseTexture);
        this.DrawBox(button, borderColour, bgColour);
        this.Label(text, textColour, button.X + Style.InnerPadding, button.Y + Style.InnerPadding);
        Raylib.EndTextureMode();

        if (showTooltip && tooltip != null)
        {
            this.Tooltip(tooltip);
        }

        this.UpdateOffsets(button.Width, button.Height);

        return wasClicked;
    }

    public bool IconButton(string icon, string? tooltip = null)
    {
        Rectangle button = new(
            x: this._xOffset,
            y: this._yOffset,
            width: Style.IconSize + Style.InnerPadding * 2,
            height: Style.IconSize + Style.InnerPadding * 2);

        (_, Color bgColour, Color borderColour, bool showTooltip, bool wasClicked) =
            this.UpdateButton(button);

        Raylib.BeginTextureMode(this._baseTexture);
        this.DrawBox(button, borderColour, bgColour);
        this.Icon(icon, this._xOffset + Style.InnerPadding, this._yOffset + Style.InnerPadding);
        Raylib.EndTextureMode();

        if (showTooltip && tooltip != null)
        {
            this.Tooltip(tooltip);
        }

        this.UpdateOffsets(button.Width, button.Height);

        return wasClicked;
    }

    private (Color, Color, Color, bool, bool) UpdateButton(Rectangle button)
    {
        Color textColour = Style.TextColour;
        Color bgColour = Style.ButtonBgColour;
        Color borderColour = Style.BorderColour;
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
                textColour = Style.ButtonDownTextColour;
                bgColour = Style.ButtonDownBgColour;
                borderColour = Style.BorderAltColour;
            }
            else
            {
                showTooltip = true;
            }
        }

        return (textColour, bgColour, borderColour, showTooltip, wasClicked);
    }
}
