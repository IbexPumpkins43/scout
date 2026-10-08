using System.Xml.Linq;

namespace Scout.Settings;

internal record Repository(
    string Id,
    string Name,
    string Url);

internal static class Settings
{
    private static bool _isInitialised = false;
    private static string _assetsPath = "";
    private static string _dataPath = "";
    private static List<Repository> _repositories = new();

    public static string AssetsPath
    {
        get
        {
            EnsureInitialised();
            return _assetsPath;
        }
    }

    public static string DataPath
    {
        get
        {
            EnsureInitialised();
            return _dataPath;
        }
    }

    public static void Load()
    {
        if (_isInitialised)
        {
            return;
        }

        XDocument settingsXml = XDocument.Load("/home/ptarmigan/Git/scout/Data/Settings.xml");

        // Ensure that there is a <Settings> to begin with
        XElement? settingsRoot = settingsXml.Root;
        if (settingsRoot == null || settingsRoot.Name != "Settings")
        {
            throw new SettingsException("Malformed settings root");
        }

        // Load the paths
        XElement? assetsPath = settingsRoot.Element("AssetsPath");
        if (assetsPath == null || assetsPath.IsEmpty)
        {
            throw new SettingsException("<AssetsPath> is either missing or empty");
        }
        XElement? dataPath = settingsRoot.Element("DataPath");
        if (dataPath == null || dataPath.IsEmpty)
        {
            throw new SettingsException("<DataPath> is either missing or empty");
        }

        _assetsPath = assetsPath.Value;
        _dataPath = dataPath.Value;

        // Load the repository list
        XElement? repositoriesRoot = settingsRoot.Element("Repositories");
        if (repositoriesRoot == null || repositoriesRoot.IsEmpty)
        {
            throw new SettingsException("<Repositories> is either missing or empty");
        }

        foreach (XElement repositoryRoot in repositoriesRoot.Elements("Repository"))
        {
            Repository repository = new(
                Id: repositoryRoot.Element("Id")!.Value,
                Name: repositoryRoot.Element("Name")!.Value,
                Url: repositoryRoot.Element("Url")!.Value);
            _repositories.Add(repository);
        }

        _isInitialised = true;
    }

    private static void EnsureInitialised()
    {
        if (!_isInitialised)
        {
            throw new SettingsException("Load the settings before trying to access properties");
        }
    }
}
