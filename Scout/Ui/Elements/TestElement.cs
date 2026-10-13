using Raylib_cs;

namespace Scout.Ui.Elements;

internal class TestElement : UiNode
{
    public Color Colour;

    public TestElement(UiSize size, Color colour)
    {
        this.DesiredSize = size;
        this.Colour = colour;
    }

    public override void Render()
    {
        Raylib.DrawRectangleLinesEx(this.Bounds, 6.0f, this.Colour);
    }
}
