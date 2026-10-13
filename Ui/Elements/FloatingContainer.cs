using System.Numerics;

namespace Scout.Ui.Elements;

internal class FloatingContainer : Container
{
    private Vector2 _position;
    public Vector2 Position
    {
        get => this._position;
        set => this.SetProperty(ref this._position, value);
    }
}
