using Raylib_cs;
using Scout.UI;

namespace Scout.Scenes;

internal class ErrorScene(SceneManager sceneManager) : Scene(sceneManager)
{
    private UIManager _uiManager = new();
    private Exception _exception;

    public override void Load()
    {
        Exception? exception = this.SceneManager.GetLastSceneResult<Exception>();
        if (exception == null)
        {
            throw new InvalidDataException("No exception was recieved by ErrorScene");
        }

        this._exception = exception;
    }

    public override void Dispose()
    {
        this._uiManager.Dispose();
    }

    public override void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.Q))
        {
            Raylib.CloseWindow();
        }
    }

    public override void Render()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        this._uiManager.BeginFrame();
        this._uiManager.Label($"{this._exception.Message}");
        if (this._uiManager.LabelButton("Quit"))
        {
            Raylib.CloseWindow();
        }
        this._uiManager.EndFrame();
        Raylib.EndDrawing();
    }
}