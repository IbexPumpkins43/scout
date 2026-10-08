namespace Scout.Settings;

internal class SettingsException : Exception
{
    public SettingsException(string message) : base($"{message}")
    {
    }

    public SettingsException(string tag, string message) : base($"<{tag}> {message}")
    {
    }
}
