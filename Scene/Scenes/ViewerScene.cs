using NativeFileDialogNET;
using Raylib_cs;
using Scout.Map;
using Scout.UI;

namespace Scout.Scenes;

internal class ViewerScene(SceneManagerData data) : Scene(data)
{
    private RenderData _renderData;
    private Renderer _renderer;
    private GraphData _graphData;
    private Graph _graph;

    private int[] _path;
    private bool _pathDirty;

    /*private bool _locationSearchBoxShow;
    private string _locationSearchBox;*/

    public override void Load()
    {
        MapData? mapData = this.SceneManager.GetLastSceneResult<MapData>();
        if (mapData == null)
        {
            throw new InvalidDataException("Failed to recieve map data from LoadingScene");
        }

        this._renderData = mapData.RenderData;
        this._graphData = mapData.GraphData;
        this._renderer = new(this._renderData, this._graphData, this.UIManager.BoldFont);
        this._graph = new(this._graphData);
        this._path = Array.Empty<int>();
        this._pathDirty = false;
    }

    public override void Dispose()
    {
        this._renderer.Dispose();
        this.UIManager.Dispose();
    }

    public override void Update()
    {
        this._renderer.Update();
    }

    public override void Render()
    {
        this._renderer.Draw();
        this._renderer.DrawPath(this._path, this._pathDirty);
        if (this._pathDirty)
        {
            this._pathDirty = false;
        }

        this.UIManager.BeginFrame();
        this.UIManager.SameLine = true;
        if (this.UIManager.IconButton("open", "Open a map file..."))
        {
            this.OpenMapFile();
        }
        if (this.UIManager.IconButton("quit", "Quit"))
        {
            Raylib.CloseWindow();
        }
        if (this.UIManager.LabelButton("Randomise", "Generates a random path from A to B using Dijkstra"))
        {
            Random random = new();
            this._path = this._graph.Dijkstra(random.Next(0, 10000), random.Next(0, 10000));
            this._pathDirty = true;
        }

        /* if (this._uiManager.IconButton("search", "Search for a place..."))
        {
            this._locationSearchBoxShow ^= true;
        }
        if (this._locationSearchBoxShow)
        {
            this._uiManager.SameLine = false;
            this._uiManager.InputBox(ref this._locationSearchBox);
        } */

        this.UIManager.EndFrame();
    }

    private void OpenMapFile()
    {
        using NativeFileDialog fileDiag = new NativeFileDialog()
            .SelectFile()
            .AddFilter("OpenStreetMap Protobuf files", "osm.pbf");

        if (fileDiag.Open(out string[]? files) == DialogResult.Okay
            && files != null
            && files.Length > 0)
        {
            this.SceneManager.SetSceneResult<string>(files[0]);
            this.SceneManager.SwitchTo<LoadingScene>();
        }
    }
}
