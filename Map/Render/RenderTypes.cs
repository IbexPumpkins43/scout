using Raylib_cs;

namespace Scout.Map;

internal readonly record struct BoundBox(
    float MinX,
    float MaxX,
    float MinY,
    float MaxY);

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
    float Thickness);

internal readonly record struct DrawableRoad(
    int Start,
    int Count,
    BoundBox Bounds,
    RoadType Type);

internal enum PlaceType
{
    Country,
    City,
    Town,
    Village,
    Hamlet
}

internal readonly record struct DrawablePlace(
    MapPosition Position,
    string Name,
    PlaceType Type);

internal readonly record struct DrawableBuilding(
    int Start,
    int Count,
    BoundBox Bounds);
