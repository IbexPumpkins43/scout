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
    public new float? DesiredSize;

    public int ChildrenCount => (this.Children ?? []).Length;

    public int HiddenContainersCount =>
        (this.Children ?? []).Count(child => child is Container && ((Container)child).Hidden);

    public int VisibleChildrenCount => this.ChildrenCount - this.HiddenContainersCount;
}
