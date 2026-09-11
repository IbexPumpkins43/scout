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

    public override void Unload()
    {
        _uiManager.Dispose();
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
        _uiManager.BeginFrame();
        _uiManager.Label($"{this._exception.Message}");
        if (_uiManager.LabelButton("Quit"))
        {
            Raylib.CloseWindow();
        }
        _uiManager.EndFrame();
        Raylib.EndDrawing();
   }
}