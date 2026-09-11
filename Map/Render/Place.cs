namespace Scout.Map;

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

internal static class Place
{
    public static PlaceType? GetType(OSMNode place)
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

    public static int GetFontSize(PlaceType type)
    {
        return type switch
        {
            PlaceType.Country => 24,
            PlaceType.City => 20,
            PlaceType.Town => 16,
            PlaceType.Village => 14,
            PlaceType.Hamlet => 12,
            _ => 12
        };
    }
}

