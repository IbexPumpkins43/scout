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

internal class UiTree : Container
{
    public UiTree(UiNode[] children)
    {
        // Container properties
        this.Orientation = ContainerOrientation.Horizontal;
        this.Fill = ContainerFill.FillBoth;
        this.BackgroundColour = Color.White;

        this.Children = children;

        this.SetParents();
        this.UpdateRootDimensions();
        this.CalculateActualSizes();
    }

    public void Update()
    {
        if (Raylib.IsWindowResized())
        {
            this.UpdateRootDimensions();
            this.CalculateActualSizes();
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
        // TODO : Replace this urgently
        if (node.GetType() == typeof(TestElement))
            Raylib.DrawRectangleLinesEx(node.Bounds, 6.0f, ((TestElement)node).Color);
        else if (node.GetType() == typeof(Container) && ((Container)node).BackgroundColour != null)
            Raylib.DrawRectangleRec(node.Bounds, ((Container)node).BackgroundColour.Value);
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

    // Makes the call further up look nicer
    private void CalculateActualSizes() => this.CalculateActualSizes(this, 0);
    // Recursive verison
    private void CalculateActualSizes(UiNode node, int nodeIdx)
    {
        // If the element is not the tree root
        if (node.Parent != null)
        {
            Container container = (Container)node.Parent;

            Rectangle newNodeBounds = container.Orientation == ContainerOrientation.Vertical
                ? this.CalculateVertContainerElement(container, node, nodeIdx)
                : this.CalculateHorizContainerElement(container, node, nodeIdx);

            node.ActualSize = newNodeBounds.Size;
            node.Bounds = newNodeBounds;
        }

        if (node.Children != null)
        {
            for (int i = 0; i < node.Children.Length; i++)
            {
                this.CalculateActualSizes(node.Children[i], i);
            }
        }
    }

    private Rectangle CalculateVertContainerElement(Container container, UiNode node, int nodeIdx)
    {
        Rectangle nodeBounds = new(
            position: container.Bounds.Position,
            size: new());

        switch (container.Fill)
        {
            case ContainerFill.None: break;
            case ContainerFill.FillHorizontally:
                nodeBounds.Width = container.Bounds.Width / (container.Children ?? []).Length;
                nodeBounds.Height = node.Bounds.Height;
                nodeBounds.X += nodeBounds.Width * nodeIdx;
                break;
            case ContainerFill.FillVertically:
                nodeBounds.Width = node.Bounds.Width;
                nodeBounds.Height = container.Bounds.Height / (container.Children ?? []).Length;
                nodeBounds.Y += nodeBounds.Height * nodeIdx;
                break;
            case ContainerFill.FillBoth:
                nodeBounds.Width = container.Bounds.Width;
                nodeBounds.Height = container.Bounds.Height / (container.Children ?? []).Length;
                nodeBounds.Y += nodeBounds.Height * nodeIdx;
                break;
        }

        return nodeBounds;
    }

    private Rectangle CalculateHorizContainerElement(Container container, UiNode node, int nodeIdx)
    {
        Rectangle nodeBounds = new(
            position: container.Bounds.Position,
            size: new());

        switch (container.Fill)
        {
            case ContainerFill.None:
                // TODO
                break;
            case ContainerFill.FillHorizontally:
                // TODO
                break;
            case ContainerFill.FillVertically:
                // TODO
                break;
            case ContainerFill.FillBoth:
                nodeBounds.Width = container.Bounds.Width / (container.Children ?? []).Length;;
                nodeBounds.Height = container.Bounds.Height;
                nodeBounds.X += nodeBounds.Width * nodeIdx;
                break;
        }

        return nodeBounds;
    }

    private void CalculateRougeElement(UiNode node, int nodeIdx)
    {
        Console.WriteLine($"{nodeIdx}");

        if (node.Parent == null)
        {
            return;
        }

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
