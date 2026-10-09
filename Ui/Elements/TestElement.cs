using Raylib_cs;

namespace Scout.Ui.Elements;

internal class TestElement : UiNode
{
    public Color Color;

    public TestElement(int w, int h, Color color)
    {
        this.DesiredSize = new(w, h);
        this.Color = color;
    }
}
