namespace Scout.Scenes;

internal abstract class Scene(SceneManager sceneManager)
{
    protected SceneManager SceneManager { get; } = sceneManager;

    public virtual void Load() {}
    public virtual void Unload() {}
    public virtual void Update() {}
    public virtual void Render() {}
}