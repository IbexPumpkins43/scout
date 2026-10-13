using Scout.Ui.Elements;

namespace Scout.Ui.Screen.Screens;

public class NewUserScreen : IScreen
{
    public void Load()
    {
        UiManager.Tree = new(
        [
            new Container()
            {
                Orientation = ContainerOrientation.Horizontal,
                Border = false,
                Children =
                [
                    new Label("Username"),
                    new InputBox()
                ]
            },
            new Container()
            {
                Orientation = ContainerOrientation.Horizontal,
                Border = false,
                Children =
                [
                    new Label("Password"),
                    new InputBox()
                    {
                        HideText = true
                    }
                ]
            }
        ])
        {
            Orientation = ContainerOrientation.Vertical,

        };
    }

    public void Dispose()
    {
    }

    public void Update()
    {
    }
}
