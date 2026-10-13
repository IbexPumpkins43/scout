namespace Scout.Map;

internal readonly record struct DrawableBuilding(
    int Start,
    int Count,
    BoundBox Bounds);
