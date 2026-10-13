using OSMPBF;

namespace Scout.Map.Osm;

internal abstract record OsmBlock(long Index);

internal sealed record OsmHeaderBlock(
    long Index,
    HeaderBlock Data)
    : OsmBlock(Index);

internal sealed record OsmDataBlock(
    long Index,
    PrimitiveBlock Data)
    : OsmBlock(Index)
{
    private readonly string?[] _stringCache = new string?[Data.Stringtable.S.Count];

    public string GetString(int index)
    {
        return this._stringCache[index] ??= this.Data.Stringtable.S[index].ToStringUtf8();
    }
}

