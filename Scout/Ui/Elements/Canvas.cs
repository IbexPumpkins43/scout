using System.Numerics;
using Raylib_cs;

namespace Scout.Ui.Elements;

internal class Canvas : UiNode, IDisposable
{
    public event Action<float, float>? OnResize;
    public event Action<MouseButton, Vector2, Vector2>? OnMouseDown;
    public event Action<float>? OnMouseScroll;
    public event Action? OnRerender;

    private RenderTexture2D _canvasTexture;
    private Rectangle _canvasTextureSource;

    public void Dispose()
    {
        Raylib.UnloadRenderTexture(this._canvasTexture);
    }

    public override void Initialise()
    {
        // Forward the events only if there are occuring within the canvas
        this.Root.OnMouseDown += (button, position, delta) =>
        {
            if (Raylib.CheckCollisionPointRec(position, this.Bounds))
            {
                this.OnMouseDown?.Invoke(button, position, delta);
            }
        };
        this.Root.OnMouseScroll += (wheel, position, delta) =>
        {
            if (Raylib.CheckCollisionPointRec(position, this.Bounds))
            {
                this.OnMouseScroll?.Invoke(wheel);
            }
        };
    }

    public override void Measure()
    {
        this.DesiredSize = UiSize.Flexible;
    }

    public override void UpdateLayout()
    {
        // Recreate the texture only if this elements bounds have changed
        if (this.Bounds.Size != this._canvasTexture.Texture.Dimensions)
        {
            Raylib.UnloadRenderTexture(this._canvasTexture);

            this._canvasTexture =
                Raylib.LoadRenderTexture((int)this.Bounds.Width, (int)this.Bounds.Height);
            if (!Raylib.IsRenderTextureValid(this._canvasTexture))
            {
                throw new UiManagerException("Failed to recreate the render texture for a canvas");
            }

            // Render textures are upside down so height needs inverting
            this._canvasTextureSource = new(
                x: 0.0f,
                y: 0.0f,
                width: this._canvasTexture.Texture.Width,
                height: -this._canvasTexture.Texture.Height);

            this.OnResize?.Invoke(this.Bounds.Width, this.Bounds.Height);
        }
    }

    public override void Render()
    {
        Raylib.DrawTextureRec(
            this._canvasTexture.Texture,
            this._canvasTextureSource,
            this.Bounds.Position,
            Color.White);
    }

    // This method is here just simple to avoid excessive GPU commands so that it can be updated
    // only when needed
    public void Invalidate()
    {
        Raylib.BeginDrawing();
        Raylib.BeginTextureMode(this._canvasTexture);

        Raylib.ClearBackground(UiManager.ColourScheme.Background);

        this.OnRerender?.Invoke();

        Raylib.EndTextureMode();
        Raylib.EndDrawing();
    }
}
