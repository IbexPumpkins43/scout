using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui.Screen.Screens;

public class TestScreen : IScreen
{
    public void Load()
    {
        UiManager.Tree = new(
        [
            new Container()
            {
                Orientation = ContainerOrientation.Horizontal,
                Fill = ContainerFill.FillBoth,
                Children =
                [
                    new Container()
                    {
                        Orientation = ContainerOrientation.Vertical,
                        Fill = ContainerFill.FillVertically,
                        Children =
                        [
                            new TestElement(50, 50, Color.SkyBlue),
                            new TestElement(50, 50, Color.Lime),
                            new TestElement(50, 50, Color.Orange),
                        ]
                    },
                    new Container()
                    {
                        Orientation = ContainerOrientation.Vertical,
                        Fill = ContainerFill.FillHorizontally,
                        Children =
                        [
                            new TestElement(100, 100, Color.Green),
                            new TestElement(100, 100, Color.Blue),
                            new TestElement(100, 100, Color.Magenta),
                        ]
                    },
                    new Container()
                    {
                        Orientation = ContainerOrientation.Vertical,
                        Fill = ContainerFill.FillBoth,
                        Children =
                        [
                            new TestElement(100, 100, Color.DarkPurple),
                            new TestElement(100, 100, Color.Violet),
                            new TestElement(100, 100, Color.Purple),
                        ]
                    },
                ]
            }
        ]);
    }

    public void Dispose() { }
    public void Update() { }
    public void Render() { }
}
