using Raylib_cs;

namespace Scout.Ui.Style.Colour;

internal class LightMode : ColourScheme
{
    public LightMode()
    {
        this.TextColour = Color.Black;
        this.TextColourAlt = Color.LightGray;

        this.ButtonBackgroundColour = Color.White;
        this.ButtonBackgroundAltColour = Color.LightGray;
        this.ButtonDownBackgroundColour = Color.Blue;
        this.ButtonDownTextColour = Color.White;

        this.ListBackgroundColourA = Color.White;
        this.ListBackgroundColourB = Color.LightGray;
        this.ListItemSelectedBackgroundColor = Color.Blue;
        this.ListTextColour = Color.Black;
        this.ListItemSelectedTextColour = Color.White;

        this.TooltipBackgroundColour = Color.Yellow;

        this.BorderColour = Color.Black;
        this.BorderColourAlt = Color.Gray;
        this.Background = Color.White;
    }
}
