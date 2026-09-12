namespace Scout.Scenes;

internal abstract class Scene(SceneManager sceneManager) : IDisposable
{
    protected SceneManager SceneManager { get; } = sceneManager;

    public virtual void Load() { }
    public virtual void Dispose() { }
    public virtual void Update() { }
    public virtual void Render() { }
}