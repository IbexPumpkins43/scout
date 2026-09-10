namespace Scout;

internal class Scene
{
    protected SceneManager SceneManager { get; }

    protected Scene(SceneManager sceneManager)
    {
        this.SceneManager = sceneManager;
    }

    public virtual void Load() {}
    public virtual void Unload() {}
    public virtual void Update() {}
    public virtual void Render() {}
}