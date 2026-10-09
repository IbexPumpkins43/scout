using Raylib_cs;

namespace Scout.Ui.Elements;

internal class TestElement : UiNode
{
    public Color Color;

    public TestElement(UiSize size, Color color)
    {
        this.DesiredSize = size;
        this.Bounds = new(0, 0, size.Width.Value, size.Height.Value);
        this.Color = color;
    }
}
