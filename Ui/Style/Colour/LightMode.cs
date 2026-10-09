using Raylib_cs;

namespace Scout.Ui.Style.Colour;

internal class LightMode : IColourScheme
{
    public static Color TextColour = Color.Black;
    public static Color TextColourAlt = Color.LightGray;

    public static Color ButtonBackgroundColour = Color.White;
    public static Color ButtonBackgroundAltColour = Color.LightGray;
    public static Color ButtonDownBackgroundColour = Color.Blue;
    public static Color ButtonDownTextColour = Color.White;

    public static Color ListBackgroundColourA = Color.White;
    public static Color ListBackgroundColourB = Color.LightGray;
    public static Color ListItemSelectedBackgroundColor = Color.Blue;
    public static Color ListTextColour = Color.Black;
    public static Color ListItemSelectedTextColour = Color.White;

    public static Color TooltipBackgroundColour = Color.Yellow;

    public static Color BorderColour = Color.Black;
    public static Color BorderColourAlt = Color.Gray;
    public static Color Background = Color.White;
}
