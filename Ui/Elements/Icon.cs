using System.Numerics;
using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Icon : UiNode
{
    public required string Name;
    public float Scale;

    private Rectangle _iconSource;
    private Rectangle _iconDestination;

    public override void UpdateLayout()
    {
        this._iconSource = UiManager.AssetsManager.IconsLookup[this.Name];
        this._iconDestination = new(
            x: this.Bounds.X + (this.Bounds.Width / 2) - (this._iconDestination.Width / 2),
            y: this.Bounds.Y + (this.Bounds.Height / 2) - (this._iconDestination.Height / 2),
            width: this._iconSource.Width * this.Scale,
            height: this._iconSource.Height * this.Scale);
        this.DesiredSize = new UiSize(this._iconDestination.Width, this._iconDestination.Height);
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
