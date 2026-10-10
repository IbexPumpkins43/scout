using System.Numerics;
using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Icon : UiNode
{
    public string Name;
    public float Scale = 1.0f;
    public UiDesiredSizeMode DesiredSizeMode = UiDesiredSizeMode.FitToContent;

    private Rectangle _iconSource;
    private Rectangle _iconDestination;
    private Vector2 _iconSize;

    public Icon(string name)
    {
        this.Name = name;
    }

    public override void Measure()
    {
        this._iconSource = UiManager.AssetsManager.IconsLookup[this.Name];

        this._iconSize = new(
            this._iconSource.Width * this.Scale,
            this._iconSource.Height * this.Scale);

        this.DesiredSize = this.DesiredSizeMode switch
        {
            UiDesiredSizeMode.FitToContent => new(this._iconSize.X, this._iconSize.Y),
            UiDesiredSizeMode.FillParent => UiSize.Flexible
        };
    }

    public override void UpdateLayout()
    {
        this._iconDestination = new(
            x: this.Bounds.X + (this.Bounds.Width - this._iconSize.X) / 2,
            y: this.Bounds.Y + (this.Bounds.Height - this._iconSize.Y) / 2,
            width: this._iconSize.X,
            height: this._iconSize.Y
        );
    }

    public override void Render()
    {
        Raylib.DrawTexturePro(
            UiManager.AssetsManager.IconsTilemap,
            this._iconSource,
            this._iconDestination,
            new(0.0f, 0.0f),
            0.0f,
            Color.White);
    }
}
