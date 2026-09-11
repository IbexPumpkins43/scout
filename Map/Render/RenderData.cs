using System.Numerics;

namespace Scout.Map;

internal class RenderData
{
    public Vector2[] Points { get; private set; } = [];
    public DrawableRoad[] Roads { get; private set; } = [];
    public DrawablePlace[] Places { get; private set; } = [];
    public DrawableBuilding[] Buildings { get; private set; } = [];

    public void Build(MapNodePositions nodePositions, MapRoads allRoads)
    {
        this.BuildRoads(nodePositions, allRoads);   
    }    

    private void BuildRoads(MapNodePositions nodePositions, MapRoads allRoads) 
    {
        List<Vector2> allPoints = new();
        List<DrawableRoad> roads = new();

        foreach (OSMWay road in allRoads)
        {       
            int start = allPoints.Count;

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (long nodeId in road.NodeIds)
            {
                if (!nodePositions.TryGetValue(nodeId, out MapPosition node))
                {
                    continue;
                }

                Vector2 point = new((float)node.X, (float)node.Y);
                allPoints.Add(point);

                minX = MathF.Min(minX, point.X);
                maxX = MathF.Max(maxX, point.X);
                minY = MathF.Min(minY, point.Y);
                maxY = MathF.Max(maxY, point.Y);
            }

            int count = allPoints.Count - start;
            if (count >= 2)
            {
                RoadType roadType = this.GetRoadType(road);
                BoundBox bounds = new(
                    MinX: minX,
                    MaxX: maxX,
                    MinY: minY,
                    MaxY: maxY);
                DrawableRoad drawableRoad = new(
                    Start: start,
                    Count: count,
                    Bounds: bounds,
                    Type: roadType);
                roads.Add(drawableRoad);
            }
            else
            {
                // Discard an invalid road
                allPoints.RemoveRange(start, count);
            }
        }

        // TODO :  A big memory spike occurs here as both the list and the array exist at the 
        //         same time
        this.Points = allPoints.ToArray();
        this.Roads = roads.ToArray();
    }

    private RoadType GetRoadType(OSMWay road)
    {
        return road.Tags.GetValueOrDefault("highway") switch
        {
            "motorway" => RoadType.Motorway,
            "motorway_link" => RoadType.MotorwayLink,
            "trunk" => RoadType.Trunk,
            "trunk_link" => RoadType.TrunkLink,
            "primary" => RoadType.Primary,
            "primary_link" => RoadType.PrimaryLink,
            "secondary" => RoadType.Secondary,
            "secondary_link" => RoadType.SecondaryLink,
            "tertiary" => RoadType.Tertiary,
            "tertiary_link" => RoadType.TertiaryLink,
            "residential" => RoadType.Residential,
            "unclassified" => RoadType.Unclassified,
            "service" => RoadType.Service,
            "living_street" => RoadType.LivingStreet,
            _ => RoadType.Other
        };
    }
}

