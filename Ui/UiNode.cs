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

    public Container? Parent;

    public virtual void Measure() {}
    public virtual void UpdateLayout() {}
    public virtual void Render() {}
}
