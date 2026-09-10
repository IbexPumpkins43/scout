namespace Scout;

internal class ViewerScene(SceneManager sceneManager) : Scene(sceneManager)
{
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

    public override void Update()
    {
        this._renderer.Update();
    }

    public override void Render()
    {
        this._renderer.Draw();
    }
}