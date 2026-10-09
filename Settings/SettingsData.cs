using System.Xml.Linq;

namespace Scout.Settings;

internal static class SettingsData
{
    private static bool _loaded;

    public static string? AssetsPath;
    public static string? DataPath;
    public static string? ColorScheme;

    // TODO : Parse repositories into a list

    public static void Load()
    {
        if (_loaded)
        {
            return;
        }

        // TODO : Improve this path situation
        XDocument settingsXml = XDocument.Load("/home/ptarmigan/Git/scout/Data/Settings.xml");

        // Ensure that there is a <Settings> to begin with
        XElement? settingsRoot = settingsXml.Root;
        if (settingsRoot == null || settingsRoot.Name != "Settings")
        {
            throw new SettingsException("Settings", "Malformed settings root");
        }

        LoadPaths(settingsRoot);
        LoadColourScheme(settingsRoot);
        LoadRepositories(settingsRoot);

        _loaded = true;
    }

    private static void LoadPaths(XElement settingsRoot)
    {
        XElement? assetsPath = settingsRoot.Element("AssetsPath");
        if (assetsPath == null || assetsPath.IsEmpty)
        {
            throw new SettingsException("AssetsPath", "missing or empty");
        }

        XElement? dataPath = settingsRoot.Element("DataPath");
        if (dataPath == null || dataPath.IsEmpty)
        {
            throw new SettingsException("DataPath", "missing or empty");
        }

        AssetsPath = assetsPath.Value;
        DataPath = dataPath.Value;
    }

    private static void LoadColourScheme(XElement settingsRoot)
    {
        XElement? colourScheme = settingsRoot.Element("ColourScheme");
        if (colourScheme == null || colourScheme.IsEmpty)
        {
            throw new SettingsException("ColourScheme", "missing or empty");
        }
        if (colourScheme.Value != "DarkMode" || colourScheme.Value != "LightMode")
        {
            throw new SettingsException("ColourScheme", "only values are DarkMode or LightMode");
        }

        ColorScheme = colourScheme.Value;
    }

    private static void LoadRepositories(XElement settingsRoot)
    {
    }
}
