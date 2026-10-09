using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui.Screen.Screens;

public class TestScreen : IScreen
{
    public void Load()
    {
        UiManager.Tree = new(
        [
            new TestElement(100, 100, Color.Red)
            {
                Children =
                [
                    new TestElement(50, 50, Color.SkyBlue),
                    new TestElement(50, 50, Color.Lime),
                    new TestElement(50, 50, Color.Orange),
                ]
            },
            new TestElement(100, 100, Color.Green),
            new TestElement(100, 100, Color.Blue),
            new TestElement(100, 100, Color.Magenta),
        ]);
    }

    public void Dispose() { }
    public void Update() { }
    public void Render() { }
}
