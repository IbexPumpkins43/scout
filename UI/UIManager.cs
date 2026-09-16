using System.Numerics;
using System.Text.Json;
using Raylib_cs;

namespace Scout.UI;

internal partial class UIManager : IDisposable
{
    private RenderTexture2D _baseTexture = Raylib.LoadRenderTexture(
        Raylib.GetScreenWidth(),
        Raylib.GetScreenHeight());
    private RenderTexture2D _tooltipTexture = Raylib.LoadRenderTexture(
        Raylib.GetScreenWidth(),
        Raylib.GetScreenHeight());

    private readonly Texture2D _icons = Raylib.LoadTexture("Assets/icons.png");
    private readonly Dictionary<string, Vector2> _iconsLookup;

    private int _xOffset;
    private int _yOffset;

    public readonly Font RegularFont = Raylib.LoadFontEx("Assets/NotoSans-Regular.ttf", 128, null, 0);
    public readonly Font BoldFont = Raylib.LoadFontEx("Assets/NotoSans-Bold.ttf", 128, null, 0);

    public bool SameLine;

    public UIManager()
    {
        string json = File.ReadAllText("Assets/icons.json");

        JsonSerializerOptions options = new();
        options.IncludeFields = true;

        Dictionary<string, Vector2>? iconsLookup =
            JsonSerializer.Deserialize<Dictionary<string, Vector2>>(json, options);
        if (iconsLookup == null)
        {
            throw new InvalidDataException("Failed to deserialise the icon position lookup JSON");
        }

        this._iconsLookup = iconsLookup;

        Raylib.SetTextureFilter(this.RegularFont.Texture, TextureFilter.Bilinear);
        Raylib.SetTextureFilter(this.BoldFont.Texture, TextureFilter.Bilinear);
    }

    public void Dispose()
    {
        Raylib.UnloadFont(this.BoldFont);
        Raylib.UnloadFont(this.RegularFont);
        Raylib.UnloadTexture(this._icons);
        Raylib.UnloadRenderTexture(this._baseTexture);
        Raylib.UnloadRenderTexture(this._tooltipTexture);
    }

    public void BeginFrame()
    {
        if (Raylib.IsWindowResized())
        {
            this._baseTexture = Raylib.LoadRenderTexture(
                Raylib.GetScreenWidth(),
                Raylib.GetScreenHeight());
            this._tooltipTexture = Raylib.LoadRenderTexture(
                Raylib.GetScreenWidth(),
                Raylib.GetScreenHeight());
        }

        Raylib.BeginTextureMode(this._baseTexture);
        Raylib.ClearBackground(new Color(0, 0, 0, 0));
        Raylib.EndTextureMode();

        Raylib.BeginTextureMode(this._tooltipTexture);
        Raylib.ClearBackground(new Color(0, 0, 0, 0));
        Raylib.EndTextureMode();

        this._xOffset = Style.OuterPadding;
        this._yOffset = Style.OuterPadding;

        this.SameLine = false;

    }

    public void EndFrame()
    {
        Raylib.DrawTextureRec(
            this._baseTexture.Texture,
            new(0, 0, this._baseTexture.Texture.Width, -this._baseTexture.Texture.Height),
            new(0, 0),
            Color.White);
        Raylib.DrawTextureRec(
            this._tooltipTexture.Texture,
            new(0, 0, this._tooltipTexture.Texture.Width, -this._tooltipTexture.Texture.Height),
            new(0, 0),
            Color.White);
    }
}
