using Scout.Ui.Elements;

namespace Scout.Ui.Screen.Screens;

public class SignUpScreen : IScreen
{
    private Form _signUpForm;
    private LabelButton _signInButton;

    public void Load()
    {
        this._signInButton = new("Or sign-in...");
        this._signInButton.OnClick += () => UiManager.ScreenManager.SwitchToScreen<SignInScreen>();

        this._signUpForm = new Form(
            title: "Sign-up",
            fields:
            [
                ("Username", new InputBox()),
                ("Password", new InputBox(){ HideText = true }),
                ("Password again", new InputBox(){ HideText = true })
            ],
            submitButtonText: "Sign-up",
            additionalButtons: [this._signInButton]);
        this._signUpForm.OnSubmit += (fields) => fields.ToList().ForEach((field) => Console.WriteLine($"{field.Key}: {field.Value}"));

        UiManager.Tree = new([this._signUpForm]);
    }

    public void Dispose()
    {
    }

    public void Update()
    {
    }
}
