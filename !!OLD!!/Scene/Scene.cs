using Scout.UI;

namespace Scout.Scenes;

internal abstract class Scene(SceneManagerData data) : IDisposable
{
    protected SceneManager SceneManager { get; } = data.SceneManager;
    protected UiManager UiManager { get; } = data.UiManager;

    public virtual void Load() { }
    public virtual void Dispose() { }
    public virtual void Update() { }
    public virtual void Render() { }
}
