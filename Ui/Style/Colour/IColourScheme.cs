using Raylib_cs;

namespace Scout.Ui.Style.Colour;

internal abstract class ColourScheme
{
    public Color TextColour { get; protected set; }
    public Color TextColourAlt { get; protected set; }

    public Color ButtonBackgroundColour { get; protected set; }
    public Color ButtonBackgroundAltColour { get; protected set; }
    public Color ButtonDownBackgroundColour { get; protected set; }
    public Color ButtonDownTextColour { get; protected set; }

    public Color ListBackgroundColourA { get; protected set; }
    public Color ListBackgroundColourB { get; protected set; }
    public Color ListItemSelectedBackgroundColor { get; protected set; }
    public Color ListTextColour { get; protected set; }
    public Color ListItemSelectedTextColour { get; protected set; }

    public Color TooltipBackgroundColour { get; protected set; }

    public Color BorderColour { get; protected set; }
    public Color BorderColourAlt { get; protected set; }
    public Color Background { get; protected set; }
}
