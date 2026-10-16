using Raylib_cs;
using Scout.Ui.Elements;

namespace Scout.Ui.Screen.Screens;

public class SignInScreen : IScreen
{
    private Form _signInForm;
    private LabelButton _signUpButton;

    public void Load()
    {
        this._signUpButton = new("Or sign-up...");
        this._signUpButton.OnClick += () => UiManager.ScreenManager.SwitchToScreen<SignUpScreen>();

        this._signInForm = new Form(
            title: "Sign-in",
            fields:
            [
                ("Username", new InputBox()),
                ("Password", new InputBox(){ HideText = true }),
            ],
            submitButtonText: "Sign-in",
            additionalButtons: [this._signUpButton]);
        this._signInForm.OnSubmit += (fields) => fields.ToList().ForEach((field) => Console.WriteLine($"{field.Key}: {field.Value}"));

        UiManager.Tree = new([this._signInForm]);
    }

    public void Dispose()
    {
    }

    public void Update()
    {
    }
}
