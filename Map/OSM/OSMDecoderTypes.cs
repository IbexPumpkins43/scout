using Google.Protobuf.Collections;
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
    : OSMBlock(Index)
{
    private readonly string?[] _stringCache = new string?[Data.Stringtable.S.Count];

    public string GetString(int index)
    {
        return this._stringCache[index] ??= this.Data.Stringtable.S[index].ToStringUtf8();
    }
}

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


