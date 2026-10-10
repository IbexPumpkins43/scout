using System.Numerics;
using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Icon : UiNode
{
    public required string Name;
    public float Scale = 1.0f;

    private Rectangle _iconSource;
    private Rectangle _iconDestination;

    public override void Measure()
    {
        this._iconSource = UiManager.AssetsManager.IconsLookup[this.Name];

        this.DesiredSize = new(
            this._iconSource.Width * this.Scale,
            this._iconSource.Height * this.Scale
        );
    }

    public override void UpdateLayout()
    {
        this._iconDestination = new(
            x: this.Bounds.X + (this.Bounds.Width - this.DesiredSize.Width.Value) / 2,
            y: this.Bounds.Y + (this.Bounds.Height - this.DesiredSize.Height.Value) / 2,
            width: this.DesiredSize.Width.Value,
            height: this.DesiredSize.Height.Value
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
