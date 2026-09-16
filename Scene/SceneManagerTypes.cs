using Scout.UI;

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

internal readonly record struct SceneManagerData(
    SceneManager SceneManager,
    UIManager UIManager);
