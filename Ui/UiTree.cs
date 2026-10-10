using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui;

internal class UiTree : Container
{
    // Elements can subscribe to these to get the events
    public event Action<Vector2>? MouseMoved;
    public event Action<MouseButton, Vector2>? MouseDown;
    public event Action<MouseButton, Vector2>? MouseUp;
    public event Action<KeyboardKey>? KeyPressed;

    private bool _isLayoutDirty = true;

    public UiTree(UiNode[] children)
    {
        // Container properties
        this.Orientation = ContainerOrientation.Horizontal;
        this.BackgroundColour = Color.White;
        this.Children = children;

        this.SetReferences();
        this.InitialiseNodes();
    }

    public void Update()
    {
        this.UpdateLayout();
        this.UpdateInput();
    }

    public void UpdateInput()
    {
        // Handle mouse movement
        Vector2 mousePosition = Raylib.GetMousePosition();
        if (Raylib.GetMouseDelta() != Vector2.Zero)
        {
            this.MouseMoved?.Invoke(mousePosition);
        }

        // Handle mouse buttons
        switch (true)
        {
            // Down
            case true when Raylib.IsMouseButtonDown(MouseButton.Left):
                this.MouseDown?.Invoke(MouseButton.Left, mousePosition);
                break;
            case true when Raylib.IsMouseButtonDown(MouseButton.Middle):
                this.MouseDown?.Invoke(MouseButton.Middle, mousePosition);
                break;
            case true when Raylib.IsMouseButtonDown(MouseButton.Right):
                this.MouseDown?.Invoke(MouseButton.Right, mousePosition);
                break;
            // Up
            case true when Raylib.IsMouseButtonUp(MouseButton.Left):
                this.MouseUp?.Invoke(MouseButton.Left, mousePosition);
                break;
            case true when Raylib.IsMouseButtonUp(MouseButton.Middle):
                this.MouseUp?.Invoke(MouseButton.Middle, mousePosition);
                break;
            case true when Raylib.IsMouseButtonUp(MouseButton.Right):
                this.MouseUp?.Invoke(MouseButton.Right, mousePosition);
                break;
        }

        // Handle keyboard
        KeyboardKey key;
        while ((key = (KeyboardKey)Raylib.GetKeyPressed()) != KeyboardKey.Null)
        {
            this.KeyPressed?.Invoke(key);
        }
    }

    public override void UpdateLayout()
    {
        if (!Raylib.IsWindowResized() && !this._isLayoutDirty)
        {
            return;
        }

        this._isLayoutDirty = false;

        this.UpdateRootDimensions();
        this.MeasureNodes();
        this.CalculateBounds();
        this.UpdateElementLayouts();
    }

    // Stop the invaldate layout from going further and mark the tree as dirty
    public override void InvalidateLayout() => this._isLayoutDirty = true;

    public override void Render()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(UiManager.ColourScheme.Background);

        this.TraverseTree(this, (node, _) =>
        {
            if (node is not UiTree)
            {
                node.Render();
            }
        }, skipHiddenContainers: true);

        Raylib.EndDrawing();
    }

    private void SetReferences()
    {
        this.TraverseTree(this, null, (node, parent) =>
        {
            node.Parent = parent;
            node.Root = this;
        });
    }

    private void InitialiseNodes() => this.TraverseTree(this, null, (node, _) => node.Initialise());

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

    private void UpdateElementLayouts()
    {
        this.TraverseTree(this, (node, _) =>
        {
            if (node is not UiTree)
            {
                node.UpdateLayout();
            }
        });
    }

    private void MeasureNodes() => this.TraverseTree(this, (node, _) => node.Measure(), true);

    // Helper method that makes traversing the tree better.
    private void TraverseTree(
        UiNode node,
        Container? parent,
        TraverseTreeAction action,
        bool backwards = false,
        bool skipHiddenContainers = false)
    {
        if (skipHiddenContainers && node is Container { Hidden: true })
        {
            return;
        }

        if (!backwards)
        {
            action(node, parent);
        }

        if (node is Container container)
        {
            foreach (UiNode child in container.Children)
            {
                this.TraverseTree(child, container, action);
            }
        }

        if (backwards)
        {
            action(node, parent);
        }
    }

    // Override to simplify calls who don't need a parent container
    private void TraverseTree(
        UiNode node,
        TraverseTreeAction action,
        bool backwards = false,
        bool skipHiddenContainers = false) =>
        this.TraverseTree(node, null, action, backwards, skipHiddenContainers);

    // Custom type to define the action
    private delegate void TraverseTreeAction(UiNode node, Container? parent = null);
}
