using Raylib_cs;

namespace Scout.Scenes;

internal class SceneManagerException : Exception
{
    public SceneManagerException(string message) 
        : base(message)
    {
    }

    public SceneManagerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

internal class SceneManager : IDisposable
{
    private Dictionary<Type, Scene> _scenes = new();
    private Scene? _currentScene;
    private bool _reload;
    private object? _lastSceneResult;

    public void Dispose()
    {
        foreach (var (_, scene) in this._scenes)
        {
            scene.Dispose();
        }
    }

    public void RegisterScene<T>() where T : Scene
    {
        if (this._scenes.ContainsKey(typeof(T)))
        {
            throw new SceneManagerException($"Scene {typeof(T)} already exists");
        }

        this._scenes.Add(typeof(T), (T)Activator.CreateInstance(typeof(T), this)!);
    }

    public void UnregisterScene<T>() where T : Scene
    {
        if (!this._scenes.ContainsKey(typeof(T)))
        {
            throw new SceneManagerException($"Scene {typeof(T)} does not exist");
        }

        this._scenes.Remove(typeof(T));
    }

    public void SwitchTo<T>() where T : Scene
    {
        if (!this._scenes.ContainsKey(typeof(T)))
        {
            throw new SceneManagerException($"Scene {typeof(T)} does not exist");
        }

        this._currentScene = this._scenes[typeof(T)];
        this._currentScene.Load(); 

        Console.WriteLine($"Switching to {this._currentScene.GetType()}");
    }

    public void Run()
    {
        if (this._currentScene == null)
        {
            throw new SceneManagerException($"Set a current scene before running");
        }

        while (!Raylib.WindowShouldClose())
        {
            this._currentScene.Update();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            this._currentScene.Render();
            Raylib.EndDrawing();
        }

    }

    public void SetSceneResult<T>(T result)
    {
        this._lastSceneResult = result;
    }

    public T? GetLastSceneResult<T>()
    {
        return this._lastSceneResult is T result ? result : default;
    }
}