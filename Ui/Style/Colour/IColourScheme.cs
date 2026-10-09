using Raylib_cs;

namespace Scout.Ui.Style.Colour;

internal interface IColourScheme
{
    public static Color TextColour;
    public static Color TextColourAlt;

    public static Color ButtonBackgroundColour;
    public static Color ButtonBackgroundAltColour;
    public static Color ButtonDownBackgroundColour;
    public static Color ButtonDownTextColour;

    public static Color ListBackgroundColourA;
    public static Color ListBackgroundColourB;
    public static Color ListItemSelectedBackgroundColor;
    public static Color ListTextColour;
    public static Color ListItemSelectedTextColour;

    public static Color TooltipBackgroundColour;

    public static Color BorderColour;
    public static Color BorderColourAlt;
    public static Color Background;
}
