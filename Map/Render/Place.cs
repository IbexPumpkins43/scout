namespace Scout.Map;

internal enum PlaceType
{
    Country,
    City,
    Town,
    Village,
    Hamlet
}

internal readonly record struct PlaceStyle(
    int FontSize,
    float MinimumZoom);

internal readonly record struct DrawablePlace(
    MapPosition Position,
    string Name,
    PlaceType Type);

internal static class Place
{
    public static PlaceType? GetPlaceType(OSMNode place)
    {
        return place.Tags.GetValueOrDefault("place") switch
        {
            "country" => PlaceType.Country,
            "city" => PlaceType.City,
            "town" => PlaceType.Town,
            "village" => PlaceType.Village,
            "hamlet" => PlaceType.Hamlet,
            _ => null
        };
    }

    public static PlaceStyle GetPlaceStyle(PlaceType type)
    {
        return type switch
        {
            PlaceType.Country => new(
                FontSize: 40,
                MinimumZoom: 0.001f),
            PlaceType.City => new(
                FontSize: 36,
                MinimumZoom: 0.002f),
            PlaceType.Town => new(
                FontSize: 32,
                MinimumZoom: 0.01f),
            PlaceType.Village => new(
                FontSize: 28,
                MinimumZoom: 0.03f),
            PlaceType.Hamlet => new(
                FontSize: 24,
                MinimumZoom: 0.08f),
            _ => new(
                FontSize: 24,
                MinimumZoom: 0.08f)
        };
    }
}

