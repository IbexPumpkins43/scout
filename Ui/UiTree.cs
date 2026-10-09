using System.Numerics;
using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui;

internal class UiNode
{
    public Vector2 DesiredSize;
    public Vector2 ActualSize;
    public Rectangle Bounds;

    public UiNode? Parent;
    public UiNode[]? Children;
}

internal class UiTree : UiNode
{
    public UiTree(UiNode[] children)
    {
        this.Children = children;

        this.SetParents();
        this.UpdateRootDimensions();
        this.RecalculateActualSizes(this, 0, 0.0f, 0.0f);
    }

    public void Update()
    {
        if (Raylib.IsWindowResized())
        {
            this.UpdateRootDimensions();
            this.RecalculateActualSizes(this, 0, 0.0f, 0.0f);
        }
    }

    // Exposed method
    public void Render()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        this.Render(this);
        Raylib.EndDrawing();
    }
    // Recursive internal method
    private void Render(UiNode node)
    {
        if (node.GetType() == typeof(TestElement))
            Raylib.DrawRectangleLinesEx(node.Bounds, 6.0f, ((TestElement)node).Color);
        else
            Raylib.DrawRectangleLinesEx(node.Bounds, 6.0f, Color.Red);

        foreach (UiNode child in node.Children ?? [])
        {
            this.Render(child);
        }
    }

    // Can't use "this" in default args so this is what I came up with
    private void SetParents() => this.SetParents(this, null);
    // Recursive version
    private void SetParents(UiNode node, UiNode? parent)
    {
        node.Parent = parent;

        foreach (UiNode child in node.Children ?? [])
        {
            this.SetParents(child, node);
        }
    }

    private void UpdateRootDimensions()
    {
        // The root of the tree is just the size of the window
        this.DesiredSize = new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        this.ActualSize = new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        this.Bounds = new(
            x: 0.0f,
            y: 0.0f,
            size: this.ActualSize);
    }

    // Recursive verison
    private void RecalculateActualSizes(UiNode node, int nodeIdx, float xOffset, float yOffset)
    {
        // If the element is not the tree root
        if (node.GetType() != typeof(UiTree) && node.Parent != null)
        {
            // If the element belongs to a container
            if (node.Parent.GetType() == typeof(Container))
            {
                Container container = (Container)node.Parent;

                float childX = container.Bounds.X; //xOffset;
                float childY = container.Bounds.Y; //yOffset;
                float childWidth = 0.0f;
                float childHeight = 0.0f;

                if (container.Orientation == ContainerOrientation.Vertical)
                {
                    switch (container.Fill)
                    {
                        case ContainerFill.None: break;
                        case ContainerFill.FillHorizontally:
                            childWidth = container.Bounds.Width / (container.Children ?? []).Length;
                            childHeight = node.Bounds.Height;
                            childX += childWidth * nodeIdx;
                            break;
                        case ContainerFill.FillVertically:
                            childWidth = node.Bounds.Width;
                            childHeight = container.Bounds.Height / (container.Children ?? []).Length;
                            childY += childHeight * nodeIdx;
                            break;
                        case ContainerFill.FillBoth:
                            childWidth = container.Bounds.Width;
                            childHeight = container.Bounds.Height / (container.Children ?? []).Length;
                            childY += childHeight * nodeIdx;
                            break;
                    }
                }
                else
                {
                    switch (container.Fill)
                    {
                        case ContainerFill.None: break;
                        case ContainerFill.FillHorizontally: break;
                        case ContainerFill.FillVertically: break;
                        case ContainerFill.FillBoth:
                            childWidth = container.Bounds.Width / (container.Children ?? []).Length;;
                            childHeight = container.Bounds.Height;
                            childX += childWidth * nodeIdx;
                            break;
                    }
                }

                node.ActualSize = new(childWidth, childHeight);
                node.Bounds = new(
                    x: childX,
                    y: childY,
                    size: node.ActualSize);
            }
            // If the element does not belong to a container
            else
            {
                float parentWidth = node.Parent.Bounds.Width;
                float parentHeight = node.Parent.Bounds.Height;

                float childWidth = parentWidth / (node.Parent.Children ?? []).Length;

                node.ActualSize = new(childWidth, parentHeight);
                node.Bounds = new(
                    x: childWidth * nodeIdx,
                    y: 0.0f,
                    size: node.ActualSize);
            }
        }
        // If the element is the tree root
        else
        {
            xOffset = node.Bounds.X;
            yOffset = node.Bounds.Y;
        }

        if (node.Children != null)
        {
            for (int i = 0; i < node.Children.Length; i++)
            {
                this.RecalculateActualSizes(node.Children[i], i, xOffset, yOffset);
            }
        }
    }
}
