using Raylib_cs;

namespace Scout.Ui.Elements;

internal enum ContainerOrientation
{
    Vertical,
    Horizontal
}

internal class Container : UiNode
{
    public ContainerOrientation Orientation;
    public Color? BackgroundColour;
    public bool Hidden;
    public bool IsEventDeadEnd;

    public UiNode[] Children = [];

    // NOTE: LINQ experessions are used here. I have no problems with using them here
    //       because UI trees are typically not very big and these are only performed when
    //       specific events are caused.

    public void CalculateBounds()
    {
        bool isVertical = this.Orientation == ContainerOrientation.Vertical;

        UiNode[] children = this.Children
            .Where(child => child is not Container { Hidden: true })
            .ToArray();

        // Anonymous function for geting the length of the axis where the container arranges the
        // children
        UiLength GetMainLength(UiNode child) => isVertical
            ? child.DesiredSize.Height
            : child.DesiredSize.Width;

        // Anonymous function for geting the length of the axis perpendicular to where the container
        // arranges the children
        UiLength GetPerpendicularLength(UiNode child) => isVertical
            ? child.DesiredSize.Width
            : child.DesiredSize.Height;

        float availableSpace = isVertical ? this.Bounds.Height : this.Bounds.Width;
        float fixedSpace = children
            .Where(child => !GetMainLength(child).IsFlexible)
            .Sum(child => GetMainLength(child).Value);

        int flexibleCount = children.Count(child => GetMainLength(child).IsFlexible);
        float flexibleNodeSize = flexibleCount > 0
            ? Math.Max(0, availableSpace - fixedSpace) / flexibleCount
            : 0.0f;

        float mainOffset = 0.0f;

        foreach (UiNode child in children)
        {
            UiLength main = GetMainLength(child);
            UiLength perpendicular = GetPerpendicularLength(child);

            // Either use the calculated flexible size or use the value defined by the child
            // depending on whether the main axis is flexible or not
            float mainSize = main.IsFlexible ? flexibleNodeSize : main.Value;

            // Similar to the calculation above but for the perpendicular axis
            float perpendicularSize = perpendicular.IsFlexible
                ? (isVertical ? this.Bounds.Width : this.Bounds.Height)
                : perpendicular.Value;

            child.Bounds = isVertical
                // Vertical
                ? new(this.Bounds.X, this.Bounds.Y + mainOffset, perpendicularSize, mainSize)
                // Horizontal
                : new(this.Bounds.X + mainOffset, this.Bounds.Y, mainSize, perpendicularSize);
            child.ActualSize = child.Bounds.Size;

            // Update the offset
            mainOffset += mainSize;

            if (child is Container container)
            {
                container.CalculateBounds();
            }
        }
    }

    public override void Render()
    {
        if (!this.Hidden && this.BackgroundColour != null)
        {
            Raylib.DrawRectangleRec(this.Bounds, this.BackgroundColour.Value);
        }
    }
}
