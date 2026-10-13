using System.Numerics;
using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui;

// This is a type purely for deciding whether the layout engine should ignore a value or not
internal readonly record struct UiLength(float Value, bool IsFlexible)
{
    public static UiLength Fixed(float value) => new(value, false);
    public static UiLength Flexible => new(0.0f, true);

    public static implicit operator UiLength(float value) => Fixed(value);
}

// Repalces Vector2 for sizes
internal readonly record struct UiSize(UiLength Width, UiLength Height)
{
    public static UiSize Flexible => new(UiLength.Flexible, UiLength.Flexible);
}

// A simple enum that lets elements choose how to size themselves
internal enum UiDesiredSizeMode
{
    FitToContent,
    FillParent
}

internal abstract class UiNode
{
    public UiSize DesiredSize = UiSize.Flexible;
    public Vector2 ActualSize;
    public Rectangle Bounds;

    public UiTree Root;
    public Container? Parent;

    // Sends an invalidate layout request up the tree to the root
    protected virtual void InvalidateLayout() => this.Parent?.InvalidateLayout();

    // First method called after the constructor, useful for subscribing to events
    public virtual void Initialise() {}
    // Called to ensure the desired size of a node is set
    public virtual void Measure() {}
    // Called to invoke a relayout of a node after potential sizing changes
    public virtual void UpdateLayout() {}
    // Called to draw the node
    public virtual void Render() {}

    // Helper to simplify the setters
    protected bool SetProperty<T>(ref T field, T value)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        this.InvalidateLayout();

        return true;
    }
}
