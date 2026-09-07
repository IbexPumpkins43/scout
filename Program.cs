using Raylib_cs;

namespace Scout;

internal class Program
{
    private static void Main()
    {
        Raylib.InitWindow(1600, 900, "Scout");
        Raylib.SetTargetFPS(60);

        var mapData = new MapData("Assets/luxembourg.osm.pbf");
        mapData.Load();

        var mapViewer = new MapViewer(mapData);

        while (!Raylib.WindowShouldClose())
        {
            mapViewer.Update();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);
            mapViewer.Draw();
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}
