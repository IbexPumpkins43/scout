
namespace Scout;

internal class MapData
{
    private string _path;

    private Dictionary<int, OSMCoordinates> _nodePositions;
    private List<OSMWay> _roads;

    public MapData(string path)
    {
        this._path = path;
        this._nodePositions = new Dictionary<int, OSMCoordinates>();
        this._roads = new List<OSMWay>();
    }

    public void Load()
    {
        using var pbfReader = new PBFReader(_path);
        pbfReader.Open();

        var osmDecoder = new OSMDecoder();
        
        // A valid OSM PBF must begin with an OSMHeader block
        var pbfHeader = pbfReader.ReadNext();
        if (pbfHeader != null)
        {
            var osmHeader = osmDecoder.Parse(pbfHeader);
            if (osmHeader.GetType() != typeof(OSMHeaderBlock))
            {
                
            }
        }

        var block = pbfReader.ReadNext();
        while (block != null)
        {
            block = pbfReader.ReadNext();
        }
    }
}