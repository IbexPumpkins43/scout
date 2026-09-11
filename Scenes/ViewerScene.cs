using NativeFileDialogNET;
using Raylib_cs;
using Scout.Map;
using Scout.UI;

namespace Scout.Scenes;

internal class ViewerScene(SceneManager sceneManager) : Scene(sceneManager)
{
    private UIManager _uiManager = new();
    private RenderData _renderData;
    private Renderer _renderer;
    private GraphData _graphData;
    private Graph _graph;

    public override void Load()
    {
        MapData? mapData = this.SceneManager.GetLastSceneResult<MapData>();
        if (mapData == null)
        {
            throw new InvalidDataException("Failed to recieve map data from LoadingScene");
        }

        this._renderData = mapData.RenderData;
        this._graphData = mapData.GraphData;
        this._renderer = new(this._renderData, this._graphData);
        this._graph = new(this._graphData);
    }

    public override void Unload()
    {
        _uiManager.Dispose();
    }

    public override void Update()
    {
        this._renderer.Update();
    }

    public override void Render()
    {
        this._renderer.Draw();
        Raylib.DrawFPS(72, 8);
        this._uiManager.BeginFrame();
        this._uiManager.SameLine = true;
        if (this._uiManager.IconButton("open", "Open a map file..."))
        {
            using NativeFileDialog fileDiag = new NativeFileDialog()
                .SelectFile()
                .AddFilter("OpenStreetMap Protobuf files", "*.osm.pbf");

            if (fileDiag.Open(out string[]? files) == DialogResult.Okay && files != null)
            {
                this.SceneManager.SetSceneResult<string>(files[0]);
                this.SceneManager.SwitchTo<LoadingScene>();
            }
        }
        if (this._uiManager.IconButton("quit", "Quit"))
        {
            Raylib.CloseWindow();
        }
        _uiManager.EndFrame();
    }
}