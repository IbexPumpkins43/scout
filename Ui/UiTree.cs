using System.Numerics;
using System.Runtime.CompilerServices;
using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui;

internal class UiTree : Container
{
    // Elements can subscribe to these to get the events
    public event Action<Vector2, Vector2>? OnMouseMove;
    public event Action<MouseButton, Vector2, Vector2>? OnMouseDown;
    public event Action<MouseButton, Vector2, Vector2>? OnMouseUp;
    public event Action<float, Vector2, Vector2>? OnMouseScroll;
    public event Action<KeyboardKey>? OnKeyPress;

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
        this.UpdateWindow();
    }

    public void UpdateInput()
    {
        // Handle mouse movement
        Vector2 mousePosition = Raylib.GetMousePosition();
        Vector2 mouseDelta = Raylib.GetMouseDelta();
        if (Raylib.GetMouseDelta() != Vector2.Zero)
        {
            this.OnMouseMove?.Invoke(mousePosition, mouseDelta);
        }

        // Handle mouse scroll
        float mouseWheel = Raylib.GetMouseWheelMove();
        if (mouseWheel != 0.0f)
        {
            this.OnMouseScroll?.Invoke(mouseWheel, mousePosition, mouseDelta);
        }

        // Handle mouse buttons
        switch (true)
        {
            // Down
            case true when Raylib.IsMouseButtonDown(MouseButton.Left):
                this.OnMouseDown?.Invoke(MouseButton.Left, mousePosition, mouseDelta);
                break;
            case true when Raylib.IsMouseButtonDown(MouseButton.Middle):
                this.OnMouseDown?.Invoke(MouseButton.Middle, mousePosition, mouseDelta);
                break;
            case true when Raylib.IsMouseButtonDown(MouseButton.Right):
                this.OnMouseDown?.Invoke(MouseButton.Right, mousePosition, mouseDelta);
                break;
            // Up
            case true when Raylib.IsMouseButtonUp(MouseButton.Left):
                this.OnMouseUp?.Invoke(MouseButton.Left, mousePosition, mouseDelta);
                break;
            case true when Raylib.IsMouseButtonUp(MouseButton.Middle):
                this.OnMouseUp?.Invoke(MouseButton.Middle, mousePosition, mouseDelta);
                break;
            case true when Raylib.IsMouseButtonUp(MouseButton.Right):
                this.OnMouseUp?.Invoke(MouseButton.Right, mousePosition, mouseDelta);
                break;
        }

        // Handle keyboard
        KeyboardKey key;
        while ((key = (KeyboardKey)Raylib.GetKeyPressed()) != KeyboardKey.Null)
        {
            this.OnKeyPress?.Invoke(key);
        }
    }

    public override void UpdateLayout()
    {
        if (!this._isLayoutDirty)
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


    public void UpdateWindow()
    {
        if (Raylib.IsWindowResized())
        {
            this._isLayoutDirty = true;
            //this.WindowResized?.Invoke(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        }
    }

    public override void Render()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(UiManager.ColourScheme.Background);

        // Render normal elements
        this.TraverseTree(this, (node, _) =>
        {
            if (node is not UiTree && !IsFloating(node))
            {
                node.Render();
            }
        }, skipHiddenContainers: true);

        // Render floating elements and their children
        this.TraverseTree(this, (node, _) =>
        {
            if (IsFloating(node))
            {
                node.Render();
            }
        }, skipHiddenContainers: true);

        Raylib.EndDrawing();
    }

    private static bool IsFloating(UiNode node)
    {
        for (UiNode? current = node; current != null; current = current.Parent)
        {
            if (current is FloatingContainer)
            {
                return true;
            }
        }

        return false;
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
                this.TraverseTree(child, container, action, backwards, skipHiddenContainers);
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
