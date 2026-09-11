using Raylib_cs;

namespace Scout.Map;

internal enum RoadType
{
    Motorway,
    MotorwayLink,
    Trunk,
    TrunkLink,
    Primary,
    PrimaryLink,
    Secondary,
    SecondaryLink,
    Tertiary,
    TertiaryLink,
    Residential,
    Unclassified,
    Service,
    LivingStreet,
    Other
}

internal readonly record struct RoadStyle(
    Color Colour,
    float Thickness,
    float MinimumZoom);

internal readonly record struct DrawableRoad(
    int Start,
    int Count,
    BoundBox Bounds,
    RoadType Type);

internal static class Road
{
    public static RoadType GetRoadType(OSMWay road)
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

    public static RoadStyle GetRoadStyle(RoadType type)
    {
        return type switch
        {
            RoadType.Motorway => new(
                Colour: Color.Orange, 
                Thickness: 6.0f, 
                MinimumZoom: 0.001f),
            RoadType.MotorwayLink => new(
                Colour: Color.Orange,
                Thickness: 4.0f, 
                MinimumZoom: 0.003f),
            RoadType.Trunk => new(
                Colour: Color.Gold, 
                Thickness: 5.0f, 
                MinimumZoom: 0.001f),
            RoadType.TrunkLink => new(
                Colour: Color.Gold, 
                Thickness: 3.5f, 
                MinimumZoom: 0.003f),
            RoadType.Primary => new(
                Colour: Color.Yellow, 
                Thickness: 4.0f, 
                MinimumZoom: 0.002f),
            RoadType.PrimaryLink => new(
                Colour: Color.Yellow, 
                Thickness: 3.0f, 
                MinimumZoom: 0.005f),
            RoadType.Secondary => new(
                Colour: Color.Beige, 
                Thickness: 3.0f, 
                MinimumZoom: 0.005f),
            RoadType.SecondaryLink => new(
                Colour: Color.Beige, 
                Thickness: 2.5f, 
                MinimumZoom: 0.01f),
            RoadType.Tertiary => new(
                Colour: Color.LightGray, 
                Thickness: 2.5f, 
                MinimumZoom: 0.01f),
            RoadType.TertiaryLink => new(
                Colour: Color.LightGray, 
                Thickness: 2.0f, 
                MinimumZoom: 0.02f),
            RoadType.Residential => new(
                Colour: Color.LightGray, 
                Thickness: 1.5f, 
                MinimumZoom: 0.03f),
            RoadType.Unclassified => new(
                Colour: Color.LightGray, 
                Thickness: 1.5f, 
                MinimumZoom: 0.08f),
            RoadType.Service => new(
                Colour: Color.Gray, 
                Thickness: 1.0f, 
                MinimumZoom: 0.08f),
            RoadType.LivingStreet => new(
                Colour: Color.Gray, 
                Thickness: 1.0f, 
                MinimumZoom: 0.05f),
            _ => new(
                Colour: Color.DarkGray, 
                Thickness: 1.0f, 
                MinimumZoom: 0.05f)
        };
    }
}