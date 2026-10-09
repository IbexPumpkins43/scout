namespace Scout.Ui.Screen;

internal class ScreenManager : IDisposable
{
    private Dictionary<Type, IScreen> _screens = new();
    private IScreen? _currentScreen;
    private object? _lastScreenResult;

    public void Dispose()
    {
        foreach ((_, IScreen? scene) in this._screens)
        {
            scene.Dispose();
        }
    }

    public void RegisterScreen<T>() where T : IScreen
    {
        if (this._screens.ContainsKey(typeof(T)))
        {
            throw new UiManagerException($"Screen {typeof(T)} already exists");
        }

        this._screens.Add(
            typeof(T),
            (T)Activator.CreateInstance(typeof(T), this)!);
    }

    public void UnregisterScreen<T>() where T : IScreen
    {
        if (!this._screens.ContainsKey(typeof(T)))
        {
            throw new UiManagerException($"Screen {typeof(T)} does not exist");
        }

        this._screens.Remove(typeof(T));
    }

    public void SwitchToScreen<T>() where T : IScreen
    {
        if (!this._screens.ContainsKey(typeof(T)))
        {
            throw new UiManagerException($"Screen {typeof(T)} does not exist");
        }

        this._currentScreen = this._screens[typeof(T)];
        this._currentScreen.Load();

        Console.WriteLine($"Switching to {this._currentScreen.GetType()}");
    }

    public void SetScreenResult<T>(T result)
    {
        this._lastScreenResult = result;
    }

    public T? GetLastScreenResult<T>()
    {
        return this._lastScreenResult is T result ? result : default;
    }
}
