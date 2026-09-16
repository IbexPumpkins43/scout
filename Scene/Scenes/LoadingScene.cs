using Raylib_cs;
using Scout.Map;

namespace Scout.Scenes;

internal class LoadingScene(SceneManagerData data) : Scene(data)
{
    private string? _path;
    private Task<MapData>? _loadTask;

    public override void Load()
    {
        this._loadTask = Task.Run(() =>
        {
            this._path = this.SceneManager.GetLastSceneResult<string>();
            if (this._path == null)
            {
                this._path = "Assets/luxembourg.osm.pbf";
            }

            Console.WriteLine($"Loading {this._path}");

            using MapImporter mapImporter = new(path: this._path);
            MapData mapData = mapImporter.Import();

            return mapData;
        });
    }

    public override void Dispose()
    {
        if (this._loadTask != null)
        {
            this._loadTask.Dispose();
        }
    }

    public override void Update()
    {
        if (this._loadTask == null)
        {
            return;
        }

        if (this._loadTask.IsFaulted)
        {
            this.SceneManager.SetSceneResult<Exception>(
                this._loadTask.Exception!.GetBaseException());
            this.SceneManager.SwitchTo<ErrorScene>();
        }
        else if (this._loadTask.IsCompletedSuccessfully)
        {
            this.SceneManager.SetSceneResult<MapData>(this._loadTask.Result);
            this.SceneManager.SwitchTo<ViewerScene>();
        }
    }

    public override void Render()
    {
        const int ringSize = 20;

        float time = (float)Raylib.GetTime() * 4;
        float angle = time * 180.0f;

        Raylib.ClearBackground(Color.White);
        Raylib.DrawRing(
            new(
                Raylib.GetScreenWidth() / 2 - ringSize / 2,
                Raylib.GetScreenHeight() / 2 - ringSize / 2),
            ringSize / 2,
            ringSize,
            angle,
            angle + 240,
            32,
            Color.SkyBlue
        );
    }
}
