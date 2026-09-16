using Scout.UI;

namespace Scout.Scenes;

internal abstract class Scene(SceneManagerData data) : IDisposable
{
    protected SceneManager SceneManager { get; } = data.SceneManager;
    protected UIManager UIManager { get; } = data.UIManager;

    public virtual void Load() { }
    public virtual void Dispose() { }
    public virtual void Update() { }
    public virtual void Render() { }
}
