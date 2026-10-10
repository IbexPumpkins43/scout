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
        this.CalculateBounds();
        this.UpdateElementLayouts();
    }

    public void Update()
    {
        if (Raylib.IsWindowResized() || Raylib.IsKeyPressed(KeyboardKey.B))
        {
            this.UpdateRootDimensions();
            this.MeasureElements();
            this.CalculateBounds();
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
        if (node is Container container)
        {
            foreach (UiNode child in container.Children)
            {
                this.MeasureElements(child);
            }
        }

        node.Measure();
    }
}
