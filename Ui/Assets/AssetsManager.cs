using System.Xml.Linq;
using Raylib_cs;
using Scout.Settings;

namespace Scout.Ui.Assets;

public class AssetsManager : IDisposable
{
    public Texture2D IconsTilemap { get; private set; }
    public Dictionary<string, Rectangle> IconsLookup { get; private set; }

    private Dictionary<int, Font> _regularFontCache = new();
    private Dictionary<int, Font> _boldFontCache = new();

    public AssetsManager()
    {
        this.LoadIcons();
        this.LoadIconsLookup();
        this.BuildFontCache();
    }

    public void Dispose()
    {
        Raylib.UnloadTexture(this.IconsTilemap);

        foreach ((_, Font font) in this._regularFontCache)
        {
            Raylib.UnloadFont(font);
        }

        foreach ((_, Font font) in this._boldFontCache)
        {
            Raylib.UnloadFont(font);
        }
    }

    // Load the appropiate size regular font on the fly so it looks good
    public Font GetRegularFont(int size)
    {
        if (!this._regularFontCache.ContainsKey(size))
        {
            Console.WriteLine($"Loading regular font at {size}");

            Font regularFont =
                Raylib.LoadFontEx(SettingsData.AssetsPath + "NotoSans-Regular.ttf", size, null, 0);
            if (!Raylib.IsFontValid(regularFont))
            {
                throw new UiManagerException($"Failed to load the regular font at {size}px");
            }

            this._regularFontCache.Add(size, regularFont);
        }

        return this._regularFontCache[size];
    }

    // Load the appropiate size bold font on the fly so it looks good
    public Font GetBoldFont(int size)
    {
        if (!this._boldFontCache.ContainsKey(size))
        {
            Console.WriteLine($"Loading bold font at {size}");

            Font boldFont =
                Raylib.LoadFontEx(SettingsData.AssetsPath + "NotoSans-Regular.ttf", size, null, 0);
            if (!Raylib.IsFontValid(boldFont))
            {
                throw new UiManagerException($"Failed to load the bold font at {size}px");
            }

            this._boldFontCache.Add(size, boldFont);
        }

        return this._boldFontCache[size];
    }

    private void LoadIcons()
    {
        this.IconsTilemap = Raylib.LoadTexture(SettingsData.AssetsPath + "icons.png");
        if (!Raylib.IsTextureValid(this.IconsTilemap))
        {
            throw new UiManagerException("Failed to load the icons tilemap");
        }
    }

    private void LoadIconsLookup()
    {
        this.IconsLookup = new();

        XDocument iconsDocument = XDocument.Load(SettingsData.AssetsPath + "Icons.xml");

        XElement? iconsRoot = iconsDocument.Root;
        if (iconsDocument.Root == null || iconsRoot.Name != "Icons")
        {
            throw new UiManagerException("Icons", "Malformed or missing icons lookup root");
        }

        foreach (XElement icon in iconsRoot.Elements("Icon"))
        {
            if (!icon.HasAttributes)
            {
                throw new UiManagerException("Icon", "Missing all attributes");
            }

            XAttribute? nameAttr = icon.Attribute("Name");
            XAttribute? xAttr = icon.Attribute("X");
            XAttribute? yAttr = icon.Attribute("Y");
            XAttribute? widthAttr = icon.Attribute("Width");
            XAttribute? heightAttr = icon.Attribute("Height");
            if (nameAttr == null
                || xAttr == null
                || yAttr == null
                || widthAttr == null
                || heightAttr == null)
            {
                throw new UiManagerException("Icon", "One or more attributes are malformed");
            }

            string iconName = (string)nameAttr;
            Rectangle iconRectangle = new(
                x: (float)xAttr,
                y: (float)yAttr,
                width: (float)widthAttr,
                height: (float)heightAttr);

            this.IconsLookup.Add(iconName, iconRectangle);
        }
    }

    // This just preloads the defined fonts sizes by the style
    private void BuildFontCache()
    {
        this.GetRegularFont(Style.Font.SmallSize);
        this.GetRegularFont(Style.Font.MediumSize);
        this.GetRegularFont(Style.Font.LargeSize);

        this.GetBoldFont(Style.Font.SmallSize);
        this.GetBoldFont(Style.Font.MediumSize);
        this.GetBoldFont(Style.Font.LargeSize);
    }
}
