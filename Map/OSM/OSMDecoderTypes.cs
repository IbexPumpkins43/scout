global using OSMTags = System.Collections.Generic.Dictionary<string, string>;

using OSMPBF;

namespace Scout.Map;

internal abstract record OSMBlock(long Index);

internal sealed record OSMHeaderBlock(
    long Index,
    HeaderBlock Data)
    : OSMBlock(Index);

internal sealed record OSMDataBlock(
    long Index,
    PrimitiveBlock Data)
    : OSMBlock(Index);

internal readonly record struct OSMCoordinates(
    double Latitude,
    double Longitude);

internal sealed record OSMNode(
    long Id, 
    OSMCoordinates Coordinates,
    OSMTags Tags);

internal sealed record OSMWay(
    long Id,
    List<long> NodeIds,
    OSMTags Tags);

