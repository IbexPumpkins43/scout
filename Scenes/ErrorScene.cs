using Raylib_cs;

namespace Scout;

internal class ErrorScene(SceneManager sceneManager) : Scene(sceneManager)
{
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

    public override void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape) || Raylib.IsKeyPressed(KeyboardKey.Q))
        {
            Raylib.CloseWindow();
        }
    }
 
    public override void Render()
    {
        const int fontSize = 20;

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        Raylib.DrawText($"{this._exception.Message}", 0, 0, fontSize, Color.Red);
        Raylib.DrawText(
            "Press Esc/Q to quit", 
            0, 
            Raylib.GetScreenHeight() - fontSize, 
            fontSize, 
            Color.Black);
        Raylib.EndDrawing();
   }
}