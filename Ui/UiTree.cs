using System.Numerics;
using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui;

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
        this.MeasureElements();
        this.CalculateActualSizes();
        this.UpdateElementLayouts();
    }

    public void Update()
    {
        if (Raylib.IsWindowResized() || Raylib.IsKeyPressed(KeyboardKey.B))
        {
            this.UpdateRootDimensions();
            this.MeasureElements();
            this.CalculateActualSizes();
            this.UpdateElementLayouts();
        }
    }

    // Exposed method
    public override void Render()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(UiManager.ColourScheme.Background);
        this.Render(this);
        Raylib.EndDrawing();
    }
    // Recursive internal method
    private void Render(UiNode node)
    {
        // Skip hidden containers and their children
        if (node is Container { Hidden: true })
        {
            return;
        }

        if (node is not UiTree)
        {
            node.Render();
        }

        if (node is not Container container)
        {
            return;
        }

        foreach (UiNode child in container.Children)
        {
            this.Render(child);
        }
    }

    // Can't use "this" in default args so this is what I came up with
    private void SetParents() => this.SetParents(this, null);
    // Recursive version
    private void SetParents(UiNode node, Container? parent)
    {
        node.Parent = parent;

        if (node is not Container container)
        {
            return;
        }

        foreach (UiNode child in container.Children)
        {
            this.SetParents(child, container);
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
        // Skip hidden containers and their children
        if (node is Container { Hidden: true })
        {
            return;
        }

        // If the element is part of a container
        if (node.Parent is Container parent)
        {
            Rectangle newNodeBounds = parent.Orientation == ContainerOrientation.Vertical
                ? this.CalculateVertContainerElement(parent, node, nodeIdx)
                : this.CalculateHorizContainerElement(parent, node, nodeIdx);

            node.ActualSize = newNodeBounds.Size;
            node.Bounds = newNodeBounds;
        }

        if (node is Container container)
        {
            for (int i = 0; i < container.Children.Length; i++)
            {
                this.CalculateActualSizes(container.Children[i], i);
            }
        }
    }

    private Rectangle CalculateVertContainerElement(Container parent, UiNode node, int nodeIdx)
    {
        Rectangle nodeBounds = new(
            position: parent.Bounds.Position,
            size: new());

        float width = node.DesiredSize.Width.Value;
        float height = node.DesiredSize.Height.Value;

        if (node.DesiredSize.Height.IsFlexible)
        {
            height = parent.FlexibleCountY > 0
                ? parent.FlexibleSpaceY / parent.FlexibleCountY
                : 0.0f;
        }

        switch (parent.Fill)
        {
            case ContainerFill.None:
                nodeBounds.Width = node.DesiredSize.Width.IsFlexible ? 0.0f : width;
                nodeBounds.Height = height;
                break;
            case ContainerFill.FillHorizontally:
                nodeBounds.Width = parent.Bounds.Width;
                nodeBounds.Height = height;
                break;
            case ContainerFill.FillVertically:
                nodeBounds.Width = node.DesiredSize.Width.IsFlexible ? 0.0f : width;
                nodeBounds.Height = height;
                break;
            case ContainerFill.FillBoth:
                nodeBounds.Width = parent.Bounds.Width;
                nodeBounds.Height = height;
                break;
        }

        nodeBounds.Y += parent.GetYOffset(nodeIdx);

        return nodeBounds;
    }

    private Rectangle CalculateHorizContainerElement(Container parent, UiNode node, int nodeIdx)
    {
        Rectangle nodeBounds = new(
            position: parent.Bounds.Position,
            size: new());

        float width = node.DesiredSize.Width.Value;
        float height = node.DesiredSize.Height.Value;

        if (node.DesiredSize.Width.IsFlexible)
        {
            width = parent.FlexibleCountX > 0
                ? parent.FlexibleSpaceX / parent.FlexibleCountX
                : 0.0f;
        }

        switch (parent.Fill)
        {
            case ContainerFill.None:
                nodeBounds.Width = width;
                nodeBounds.Height = node.DesiredSize.Height.IsFlexible ? 0.0f : height;
                break;
            case ContainerFill.FillHorizontally:
                nodeBounds.Width = width;
                nodeBounds.Height = node.DesiredSize.Height.IsFlexible ? 0.0f : height;
                break;
            case ContainerFill.FillVertically:
                nodeBounds.Width = width;
                nodeBounds.Height = parent.Bounds.Height;
                break;
            case ContainerFill.FillBoth:
                nodeBounds.Width = width;
                nodeBounds.Height = parent.Bounds.Height;
                break;
        }

        nodeBounds.X += parent.GetXOffset(nodeIdx);

        return nodeBounds;
    }

    // Make the call look good
    private void UpdateElementLayouts() => this.UpdateElementLayouts(this);
    // Recursive version
    private void UpdateElementLayouts(UiNode node)
    {
        node.UpdateLayout();

        if (node is not Container container)
        {
            return;
        }

        foreach (UiNode child in container.Children)
        {
            this.UpdateElementLayouts(child);
        }
    }

    // Make the call look good
    private void MeasureElements() => this.MeasureElements(this);
    // Recursive version
    private void MeasureElements(UiNode node)
    {
        node.Measure();

        if (node is not Container container)
        {
            return;
        }

        foreach (UiNode child in container.Children)
        {
            this.MeasureElements(child);
        }
    }
}
