using Raylib_cs;
using NativeFileDialogNET;
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

    private Container _leftSidebar;
    private Container _rightSidebar;

    public void Load()
    {
        // Import the map
        this.LoadMap(SettingsData.DataPath + "Maps/monaco.osm.pbf");

        // Build the user interface
        this._leftSidebar = new()
        {
            Orientation = ContainerOrientation.Vertical,
            DesiredSize = new(500.0f, UiLength.Flexible),
            BackgroundColour = UiManager.ColourScheme.Background,
            Hidden = true
        };
        this._rightSidebar = new()
        {
            Orientation = ContainerOrientation.Vertical,
            DesiredSize = new(500.0f, UiLength.Flexible),
            BackgroundColour = UiManager.ColourScheme.Background,
            Hidden = true
        };
        UiManager.Tree = new(
        [
            new Menubar(
            [
                new("File",
                [
                    new("Open map...", this.OpenMap),
                    new("Download map...", this.DownloadMap),
                    new("Quit", () => Raylib.CloseWindow())
                ])
            ]),
            new Container()
            {
                Orientation = ContainerOrientation.Horizontal,
                Children =
                [
                    this._leftSidebar,
                    this._renderer.Canvas,
                    this._rightSidebar
                ]
            },
        ])
        {
            Orientation = ContainerOrientation.Vertical
        };
    }

    public void Dispose()
    {
        this._renderer.Dispose();
    }

    public void Update()
    {

    }

    private void LoadMap(string path)
    {
        using MapImporter mapImporter = new(path);
        MapData mapData = mapImporter.Import();

        // Setup the viewer state
        this._renderData = mapData.RenderData;
        this._graphData = mapData.GraphData;
        this._renderer = new(
            renderData: this._renderData,
            graphData: this._graphData,
            font: UiManager.AssetsManager.GetRegularFont(64));
        this._graph = new(this._graphData);
    }

    private void OpenMap()
    {
        this._leftSidebar.Hidden = false;
        this._leftSidebar.Children =
        [
            new Label("Open map")
            {
                Font = UiManager.AssetsManager.GetRegularFont(40),
                FontSize = 40
            }
        ];
    }

    private void DownloadMap() {}
}
