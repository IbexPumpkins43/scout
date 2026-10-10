using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui.Screen.Screens;

public class TestScreen : IScreen
{
    private LabelButton myButton = new LabelButton("Test button!");

    public void Load()
    {
        this.myButton.Clicked += () =>
        {
            Console.WriteLine("test");
        };

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
                BackgroundColour = Color.White,
                Hidden = true,
                DesiredSize = new(300.0f, UiLength.Flexible),
                Children =
                [
                    // Logo
                    new Container()
                    {
                        Orientation = ContainerOrientation.Horizontal,
                        DesiredSize = new(UiLength.Flexible, 80.0f),
                        Children =
                        [
                            new Label("Scout")
                            {
                                FontSize = Style.Font.LargeSize,
                                DesiredSizeMode = UiDesiredSizeMode.FillParent
                            },
                            new Icon("Search")
                            {
                                Scale = 4.0f,
                                DesiredSizeMode = UiDesiredSizeMode.FillParent
                            },
                        ]
                    },
                    myButton,
                    new TestElement(new(50, 40), Color.SkyBlue),
                    new TestElement(new(50, 40), Color.Lime),
                    new TestElement(new(50, 40), Color.Orange),
                ]
            },
            new Container()
            {
                Orientation = ContainerOrientation.Vertical,
                Children =
                [
                    new Container()
                    {
                        Orientation = ContainerOrientation.Vertical,
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
