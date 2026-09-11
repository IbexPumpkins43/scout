using Raylib_cs;

namespace Scout.Map;

internal static class RoadStyles
{
    public static RoadStyle Get(RoadType type)
    {
        return type switch
        {
            RoadType.Motorway => new(Colour: Color.Orange, Thickness: 6.0f),
            RoadType.MotorwayLink => new(Colour: Color.Orange, Thickness: 4.0f),
            RoadType.Trunk => new(Colour: Color.Gold, Thickness: 5.0f),
            RoadType.TrunkLink => new(Colour: Color.Gold, Thickness: 3.5f),
            RoadType.Primary => new(Colour: Color.Yellow, Thickness: 4.0f),
            RoadType.PrimaryLink => new(Colour: Color.Yellow, Thickness: 3.0f),
            RoadType.Secondary => new(Colour: Color.Beige, Thickness: 3.0f),
            RoadType.SecondaryLink => new(Colour: Color.Beige, Thickness: 2.5f),
            RoadType.Tertiary => new(Colour: Color.LightGray, Thickness: 2.5f),
            RoadType.TertiaryLink => new(Colour: Color.LightGray, Thickness: 2.0f),
            RoadType.Residential => new(Colour: Color.LightGray, Thickness: 1.5f),
            RoadType.Unclassified => new(Colour: Color.LightGray, Thickness: 1.5f),
            RoadType.Service => new(Colour: Color.Gray, Thickness: 1.0f),
            RoadType.LivingStreet => new(Colour: Color.Gray, Thickness: 1.0f),
            _ => new(Colour: Color.DarkGray, Thickness: 1.0f)
        };
    }
}