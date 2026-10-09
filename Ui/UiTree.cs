using System.Numerics;
using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui;

internal class UiNode
{
    public Vector2 DesiredSize;
    public Vector2 ActualSize;
    public Rectangle Bounds;

    public bool ChildrenShouldFill;

    public UiNode? Parent;
    public UiNode[]? Children;
}

internal class UiTree : UiNode
{
    public UiTree(UiNode[] children)
    {
        this.Children = children;
        this.ChildrenShouldFill = true;

        this.SetParents();
        this.UpdateRootDimensions();
        this.RecalculateActualSizes(this, 0, 0.0f);
    }

    public void Update()
    {
        if (Raylib.IsWindowResized())
        {
            this.UpdateRootDimensions();
            this.RecalculateActualSizes(this, 0, 0.0f);
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

    private void RecalculateActualSizes(UiNode node, int nodeIdx, float xOffset)
    {
        if (node.GetType() != typeof(UiTree) && node.Parent != null)
        {
            // Rectangle doesn't allow for easy expansion so this is what I have to do
            // float parentX = node.Parent.Bounds.X;
            // float parentY = node.Parent.Bounds.Y;
            float parentWidth = node.Parent.Bounds.Width;
            float parentHeight = node.Parent.Bounds.Height;

            float childWidth = parentWidth / (node.Parent.Children ?? []).Length;

            node.ActualSize = new(childWidth, parentHeight);
            node.Bounds = new(
                x: childWidth * nodeIdx,
                y: 0.0f,
                size: node.ActualSize);
        }
        else
        {
            xOffset = node.Bounds.X;
        }

        if (node.Children != null)
        {
            for (int i = 0; i < node.Children.Length; i++)
            {
                this.RecalculateActualSizes(node.Children[i], i, xOffset);
            }
        }
    }
}
