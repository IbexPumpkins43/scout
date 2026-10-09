using Raylib_cs;

namespace Scout.Ui.Elements;

internal enum ContainerOrientation
{
    Vertical,
    Horizontal
}

internal enum ContainerFill
{
    None,
    FillHorizontally,
    FillVertically,
    FillBoth
}

internal class Container : UiNode
{
    public ContainerOrientation Orientation;
    public ContainerFill Fill;
    public Color? BackgroundColour;
    public bool Hidden;
}
