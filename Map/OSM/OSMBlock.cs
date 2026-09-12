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

