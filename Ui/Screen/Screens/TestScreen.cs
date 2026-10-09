using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui.Screen.Screens;

public class TestScreen : IScreen
{
    public void Load()
    {
        // DesiredSize example
        /*UiManager.Tree = new(
        [
            new Container()
            {
                Orientation = ContainerOrientation.Vertical,
                Fill = ContainerFill.FillHorizontally,
                BackgroundColour = Color.Blue,
                DesiredSize = new(300.0f, 0.0f),
                Children = []
            },
            new Container()
            {
                Orientation = ContainerOrientation.Vertical,
                Fill = ContainerFill.FillBoth,
                BackgroundColour = Color.DarkBlue,
                Children = []
            }
        ]);*/
        // Full example
        UiManager.Tree = new(
        [
            new Container()
            {
                Orientation = ContainerOrientation.Vertical,
                Fill = ContainerFill.FillVertically,
                BackgroundColour = Color.White,
                Hidden = true,
                DesiredSize = new(300.0f, -1.0f),
                Children =
                [
                    new Label()
                    {
                        Text = "Hello world!",
                        FontSize = Style.Font.LargeSize
                    },
                    new TestElement(new(50, UiLength.Flexible), Color.SkyBlue),
                    new TestElement(new(50, UiLength.Flexible), Color.Lime),
                    new TestElement(new(50, UiLength.Flexible), Color.Orange),
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
                            new TestElement(new(UiLength.Flexible, 100.0f), Color.Green),
                            new TestElement(new(UiLength.Flexible, 100.0f), Color.Blue),
                            new TestElement(new(UiLength.Flexible, 100.0f), Color.Magenta),
                        ]
                    },
                    new Container()
                    {
                        Orientation = ContainerOrientation.Vertical,
                        Fill = ContainerFill.FillBoth,
                        BackgroundColour = Color.Black,
                        Children =
                        [
                            new TestElement(UiSize.Flexible, Color.DarkPurple),
                            new TestElement(UiSize.Flexible, Color.Violet),
                            new TestElement(UiSize.Flexible, Color.Purple),
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
