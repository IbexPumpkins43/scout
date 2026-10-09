using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui.Screen.Screens;

public class TestScreen : IScreen
{
    public void Load()
    {
        // DesiredSize example
        UiManager.Tree = new(
        [
            new Container()
            {
                Orientation = ContainerOrientation.Vertical,
                Fill = ContainerFill.FillHorizontally,
                BackgroundColour = Color.Blue,
                DesiredSize = 300.0f,
                Children = []
            },
            new Container()
            {
                Orientation = ContainerOrientation.Vertical,
                Fill = ContainerFill.FillBoth,
                BackgroundColour = Color.DarkBlue,
                Children = []
            }
        ]);
        /* Full example
        UiManager.Tree = new(
        [
            new Container()
            {
                Orientation = ContainerOrientation.Vertical,
                Fill = ContainerFill.FillVertically,
                BackgroundColour = Color.Black,
                Hidden = true,
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
                Fill = ContainerFill.FillBoth,
                Children =
                [
                    new Container()
                    {
                        Orientation = ContainerOrientation.Vertical,
                        Fill = ContainerFill.FillHorizontally,
                        BackgroundColour = Color.Gray,
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
                        BackgroundColour = Color.White,
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
        */
    }

    public void Dispose() { }
    public void Update() { }
    public void Render() { }
}
