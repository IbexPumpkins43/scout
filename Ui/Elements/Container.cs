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

    // NOTE: LINQ experession are heavily used here. I have no problems with using them here
    //       because UI trees are typically not very big and these are only performed when
    //       specific events are caused.

    public int ChildrenCount => (this.Children ?? []).Length;

    public int HiddenContainersCount =>
        (this.Children ?? []).Count(child => child is Container && ((Container)child).Hidden);

    public int VisibleContainersCount => this.ChildrenCount - this.HiddenContainersCount;

    public int FlexibleCount => (this.Children ?? []).Count(child =>
        (child is not Container || !((Container)child).Hidden) && child.DesiredSize == null);

    public float DesiredSpaceX =>
        (this.Children ?? [])
        .Where(child => child is not Container || !((Container)child).Hidden)
        .Sum(child => child.DesiredSize?.X ?? 0.0f);

    public float DesiredSpaceY => (this.Children ?? [])
        .Where(child => child is not Container || !((Container)child).Hidden)
        .Sum(child => child.DesiredSize?.Y ?? 0.0f);

    public float FlexibleSpaceX => this.Bounds.Width - this.DesiredSpaceX;
    public float FlexibleSpaceY => this.Bounds.Width - this.DesiredSpaceY;

    public float GetXOffset(int idx) =>
        (this.Children ?? [])[..idx]
        .Where(child => child is not Container || !((Container)child).Hidden)
        .Sum(child => child.Bounds.Width);

    public float GetYOffset(int idx) =>
        (this.Children ?? [])[..idx]
        .Where(child => child is not Container || !((Container)child).Hidden)
        .Sum(child => child.Bounds.Height);
}
