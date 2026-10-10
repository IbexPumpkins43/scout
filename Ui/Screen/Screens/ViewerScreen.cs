using Scout.Map;
using Scout.Map.Graph;
using Scout.Ui.Elements;
using Scout.Settings;

namespace Scout.Ui.Screen.Screens;

internal class ViewerScreen : IScreen
{
    private RenderData _renderData;
    private Renderer _renderer;
    private GraphData _graphData;
    private Graph _graph;

    public void Load()
    {
        // Import the map
        using MapImporter mapImporter = new(SettingsData.DataPath + "Maps/monaco.osm.pbf");
        MapData mapData = mapImporter.Import();

        // Setup the viewer state
        this._renderData = mapData.RenderData;
        this._graphData = mapData.GraphData;
        this._renderer = new(
            renderData: this._renderData,
            graphData: this._graphData,
            font: UiManager.AssetsManager.GetRegularFont(64));
        this._graph = new(this._graphData);

        // Build the user interface
        UiManager.Tree = new(
        [
            // Sidebar
            new Container()
            {
                Orientation = ContainerOrientation.Vertical,
                Hidden = false,
                DesiredSize = new(300.0f, UiLength.Flexible),
                Children =
                [
                    // Logo
                    new Container()
                    {
                        Orientation = ContainerOrientation.Horizontal,
                        DesiredSize = new(UiLength.Flexible, 80.0f),
                        Children =
                        [
                            new Label("Scout")
                            {
                                FontSize = Style.Font.LargeSize,
                                DesiredSizeMode = UiDesiredSizeMode.FillParent
                            },
                            new Icon("Search")
                            {
                                Scale = 4.0f,
                                DesiredSizeMode = UiDesiredSizeMode.FillParent
                            },
                        ]
                    },
                ]
            },
            this._renderer.Canvas
        ]);
    }

    public void Dispose()
    {
        this._renderer.Dispose();
    }

    public void Update() { }
    public void Render() { }
}
