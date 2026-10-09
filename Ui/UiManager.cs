using System.Xml.Linq;
using Raylib_cs;
using Scout.Settings;

namespace Scout.Ui;

internal class UiManager : IDisposable
{
    private Texture2D _iconsTilemap = Raylib.LoadTexture(SettingsData.AssetsPath + "icons.png");
    private Dictionary<string, Rectangle> _iconsTilemapLookup = new();

    private Font _regularFont = Raylib.LoadFontEx(SettingsData.AssetsPath + "NotoSans-Regular.ttf", 48, null, 0);
    private Font _boldFont = Raylib.LoadFontEx(SettingsData.AssetsPath + "NotoSans-Bold.ttf", 48, null, 0);

    public UiManager()
    {
        // Load the icons lookup document into a dictionary
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

            this._iconsTilemapLookup.Add(iconName, iconRectangle);
        }
    }

    public void Dispose()
    {
        Raylib.UnloadTexture(this._iconsTilemap);
        Raylib.UnloadFont(this._regularFont);
        Raylib.UnloadFont(this._boldFont);
    }
}
