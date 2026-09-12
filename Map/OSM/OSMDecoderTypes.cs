using Google.Protobuf.Collections;
using OSMPBF;

namespace Scout.Map;

internal readonly record struct OSMCoordinates(
    double Latitude,
    double Longitude);

internal readonly record struct OSMNode(
    long Id,
    OSMCoordinates Coordinates,
    OSMTags Tags);

internal readonly record struct OSMNodeView(
    long Id,
    OSMCoordinates Coordinates,
    OSMTagView Tags);

internal readonly record struct OSMWay(
    long Id,
    long[] NodeIds,
    OSMTags Tags);

internal readonly record struct OSMWayView(
    long Id,
    RepeatedField<long> Refs,
    OSMTagView Tags);


