namespace Scout.Map;

internal readonly record struct BoundBox(
    float MinX,
    float MaxX,
    float MinY,
    float MaxY);

internal enum RoadType
{
    Motorway,
    Trunk,
    Primary,
    Secondary,
    Tertiary,
    Residential,
    Unclassified,
    Service,
    LivingStreet,
    Other
}

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
);
