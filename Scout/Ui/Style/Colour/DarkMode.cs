using Raylib_cs;

namespace Scout.Ui.Style.Colour;

internal class DarkMode : ColourScheme
{
    public DarkMode()
    {
        this.TextColour = Color.White;
        this.TextColourAlt = Color.LightGray;

        this.ButtonBackgroundColour = Color.DarkGray;
        this.ButtonBackgroundAltColour = Color.Gray;
        this.ButtonDownBackgroundColour = Color.Blue;
        this.ButtonDownTextColour = Color.White;

        this.ListBackgroundColourA = Color.White;
        this.ListBackgroundColourB = Color.LightGray;
        this.ListItemSelectedBackgroundColor = Color.Blue;
        this.ListTextColour = Color.Black;
        this.ListItemSelectedTextColour = Color.White;

        this.TooltipBackgroundColour = Color.Yellow;

        this.BorderColour = Color.DarkGray;
        this.BorderColourAlt = Color.LightGray;

        this.Background = Color.DarkGray;
    }
}
