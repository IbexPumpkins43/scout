using Google.Protobuf.Collections;

namespace Scout.Map.Osm;

internal readonly record struct OsmCoordinates(
    double Latitude,
    double Longitude);

internal readonly record struct OsmNode(
    long Id,
    OsmCoordinates Coordinates,
    OsmTags Tags);

internal readonly record struct OsmNodeView(
    long Id,
    OsmCoordinates Coordinates,
    OsmTagView Tags);

internal readonly record struct OsmWay(
    long Id,
    long[] NodeIds,
    OsmTags Tags);

internal readonly record struct OsmWayView(
    long Id,
    RepeatedField<long> Refs,
    OsmTagView Tags);


