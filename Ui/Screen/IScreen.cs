namespace Scout.Ui.Screen;

internal interface IScreen : IDisposable
{
    public void Load();
    public void Update();
    public void Render();
}
