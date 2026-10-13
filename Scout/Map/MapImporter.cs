using Scout.Map.Osm;

namespace Scout.Map;

internal class MapImporter(string path) : IDisposable
{
    private string _path = path;

    private PbfReader _pbfReader = new(path);
    private OsmDecoder _osmDecoder = new();

    private MapNodePositions _nodePositions = new();
    private MapRoads _roads = new();
    private MapPlaces _places = new();
    private MapBuildings _buildings = new();

    public MapData Import()
    {
        this._pbfReader.Open();

        this.ValidateHeader();
        this.DecodeBlocks();
        this.ProjectNodes();

        MapData mapData = new();
        mapData.RenderData.Build(this._nodePositions, this._roads, this._places, this._buildings);
        mapData.GraphData.Build(this._nodePositions, this._roads);

        return mapData;
    }

    public void Dispose()
    {
        this._pbfReader.Dispose();
    }

    private void ValidateHeader()
    {
        PBFBlock? pbfHeader = this._pbfReader.ReadNext();
        if (pbfHeader == null)
        {
            throw new MapImporterException(this._path, "Header is missing");
        }

        OsmBlock osmHeader = this._osmDecoder.Parse(pbfHeader);
        if (osmHeader.GetType() != typeof(OsmHeaderBlock))
        {
            throw new MapImporterException(
                this._path,
                $"Expected header, but got {osmHeader.GetType().Name}");
        }
    }

    private void DecodeBlocks()
    {
        PBFBlock? pbfBlock = this._pbfReader.ReadNext();
        while (pbfBlock != null)
        {
            OsmDataBlock osmBlock = (OsmDataBlock)this._osmDecoder.Parse(pbfBlock);

            this.DecodeNodes(osmBlock);
            this.DecodeWays(osmBlock);

            pbfBlock = this._pbfReader.ReadNext();
        }
    }

    private void DecodeNodes(OsmDataBlock block)
    {
        foreach (OsmNodeView node in this._osmDecoder.DecodeNodes(block))
        {
            MapPosition mapPosition = new(
                X: node.Coordinates.Latitude,
                Y: node.Coordinates.Longitude);
            this._nodePositions.Add(node.Id, mapPosition);

            if (node.Tags.ContainsKey("place"u8))
            {
                OsmNode newNode = new(
                    Id: node.Id,
                    Coordinates: node.Coordinates,
                    Tags: node.Tags.Materialize());
                this._places.Add(newNode);
            }
        }
    }

    private void DecodeWays(OsmDataBlock block)
    {
        foreach (OsmWayView way in this._osmDecoder.DecodeWays(block))
        {
            bool isRoad = way.Tags.ContainsKey("highway"u8);
            bool isBuilding = way.Tags.ContainsKey("building"u8);

            if (!isRoad && !isBuilding)
            {
                continue;
            }

            OsmWay newWay = new(
                Id: way.Id,
                NodeIds: this._osmDecoder.DecodeNodeIds(way.Refs),
                Tags: way.Tags.Materialize());

            if (isRoad)
            {
                this._roads.Add(newWay);
            }
            else if (isBuilding)
            {
                this._buildings.Add(newWay);
            }

        }
    }

    private void ProjectNodes()
    {
        const int metresPerDegree = 111320;

        if (this._nodePositions.Count == 0)
        {
            throw new MapImporterException(this._path, "Map contains no nodes");
        }

        MapPosition origin = this._nodePositions.First().Value;
        double originX = origin.X;
        double originY = origin.Y;

        foreach ((long id, (double x, double y)) in this._nodePositions)
        {
            this._nodePositions[id] = new(
                // Use a local origin to keep projected coordinates near 0
                X:
                    (y - originY)
                    * Math.Cos(double.DegreesToRadians(originX))
                    * metresPerDegree,
                // Invert Y so north appears upward
                Y: -(x - originX) * metresPerDegree);
        }

    }
}
