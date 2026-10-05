using Raylib_cs;

namespace Scout.UI;

internal static class Style
{
    public const int LargeFontSize = 64;
    public const float LargeFontSpacing = 0.0f;
    public const int RegularFontSize = 20;
    public const float RegularFontSpacing = 0.0f;
    public const int SmallFontSize = 15;
    public const float SmallFontSpacing = 0.0f;

    public const int IconSize = 20;

    public static Color TextColour = Color.Black;
    public static Color TextColourAlt = Color.LightGray;

    public static Color ButtonBgColour = Color.White;
    public static Color ButtonBgAltColour = Color.LightGray;
    public static Color ButtonDownBgColour = Color.Blue;
    public static Color ButtonDownTextColour = Color.White;

    public const int ListMaxWidth = 600;
    public const int ListMaxItemsVisible = 15;
    public static Color ListBgColourA = Color.White;
    public static Color ListBgColourB = Color.LightGray;
    public static Color ListTextColour = Color.Black;
    public static Color ListTextColourSelected = Color.Blue;

    public static Color TooltipBgColour = Color.Yellow;

    public static Color BorderColour = Color.Black;
    public static Color BorderAltColour = Color.Gray;
    public static Color Bg = Color.White;

    public const int OuterPadding = 4;
    public const int InnerPadding = 4;

    public const int Padding = 4;
}
