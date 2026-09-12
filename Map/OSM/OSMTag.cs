using Google.Protobuf.Collections;
using OSMPBF;

namespace Scout.Map;

internal readonly record struct OSMTag(
    string Key,
    string Value);

internal readonly struct OSMTags(OSMTag[] tags)
{
    private readonly OSMTag[]? _tags = tags;

    public bool ContainsKey(string key)
    {
        return this.TryGetValue(key, out _);
    }

    public bool TryGetValue(string key, out string? value)
    {
        if (this._tags != null)
        {
            for (int i = 0; i < this._tags.Length; i++)
            {
                if (this._tags[i].Key == key)
                {
                    value = this._tags[i].Value;
                    return true;
                }
            }
        }

        value = null;
        return false;
    }

    public string? GetValueOrDefault(string key)
    {
        this.TryGetValue(key, out string? value);
        return value;
    }
}

internal readonly struct OSMTagView
{
    private readonly OSMDataBlock? _block;

    private readonly RepeatedField<uint>? _keys;
    private readonly RepeatedField<uint>? _vals;

    private readonly DenseNodes? _denseNodes;
    private readonly int _denseStart;

    public int Count { get; }

    public OSMTagView(OSMDataBlock block, RepeatedField<uint> keys, RepeatedField<uint> vals)
    {
        this._block = block;
        this._keys = keys;
        this._vals = vals;
        this.Count = keys.Count;
    }

    public OSMTagView(OSMDataBlock block, DenseNodes nodes, int start, int count)
    {
        this._block = block;

        this._denseNodes = nodes;
        this._denseStart = start;

        this.Count = count;
    }

    public bool ContainsKey(ReadOnlySpan<byte> key)
    {
        for (int i = 0; i < this.Count; i++)
        {
            int keyIndex = this.GetKeyIndex(i);
            if (this._block!.Data.Stringtable.S[keyIndex].Span.SequenceEqual(key))
            {
                return true;
            }
        }

        return false;
    }

    public OSMTags Materialize()
    {
        if (this.Count == 0)
        {
            return default;
        }

        OSMTag[] tags = new OSMTag[this.Count];

        for (int i = 0; i < tags.Length; i++)
        {
            int keyIndex = this.GetKeyIndex(i);
            int valIndex = this.GetValueIndex(i);

            tags[i] = new(
                Key: this._block!.GetString(keyIndex),
                Value: this._block.GetString(valIndex));
        }

        return new(tags);
    }

    private int GetKeyIndex(int index)
    {
        if (this._denseNodes != null)
        {
            return this._denseNodes.KeysVals[this._denseStart + index * 2];
        }

        return (int)this._keys![index];
    }

    private int GetValueIndex(int index)
    {
        if (this._denseNodes != null)
        {
            return this._denseNodes.KeysVals[this._denseStart + index * 2 + 1];
        }

        return (int)this._vals![index];
    }
}
