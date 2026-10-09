namespace Scout.Ui;

internal class UiManagerException : Exception
{
    public UiManagerException(string message) : base(message)
    {
    }

    public UiManagerException(string tag, string message) : base($"<{tag}> {message}")
    {
    }

    public UiManagerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
