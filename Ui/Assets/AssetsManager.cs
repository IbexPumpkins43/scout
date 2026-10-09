using System.Xml.Linq;
using Raylib_cs;
using Scout.Settings;

namespace Scout.Ui.Assets;

public class AssetsManager : IDisposable
{
    public Texture2D IconsTilemap { get; private set; }
    public Dictionary<string, Rectangle> IconsLookup { get; private set; }

    public Font RegularFont { get; private set; }
    public Font BoldFont { get; private set; }

    public AssetsManager()
    {
        this.LoadAssets();
        this.LoadIconsLookup();
    }

    public void Dispose()
    {
        Raylib.UnloadTexture(this.IconsTilemap);
        Raylib.UnloadFont(this.RegularFont);
        Raylib.UnloadFont(this.BoldFont);
    }

    private void LoadAssets()
    {
        this.IconsTilemap = Raylib.LoadTexture(SettingsData.AssetsPath + "icons.png");
        if (!Raylib.IsTextureValid(this.IconsTilemap))
        {
            throw new UiManagerException("Failed to load the icons tilemap");
        }

        this.RegularFont = Raylib.LoadFontEx(SettingsData.AssetsPath + "NotoSans-Regular.ttf", 48, null, 0);
        if (!Raylib.IsFontValid(this.RegularFont))
        {
            throw new UiManagerException("Failed to load the regular font");
        }

        this.BoldFont = Raylib.LoadFontEx(SettingsData.AssetsPath + "NotoSans-Bold.ttf", 48, null, 0);
        if (!Raylib.IsFontValid(this.BoldFont))
        {
            throw new UiManagerException("Failed to load the bold font");
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
}
