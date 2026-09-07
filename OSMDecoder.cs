using Google.Protobuf.Collections;
using OSMPBF;

using OSMTags = System.Collections.Generic.Dictionary<string, string>;

namespace Scout;

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
    long ID, 
    OSMCoordinates Coordinates,
    OSMTags Tags);

internal sealed record OSMWay(
    long ID,
    List<long> NodeIDs,
    OSMTags Tags);

internal class OSMDecoder
{
    public OSMBlock Parse(PBFBlock pbfBlock)
    {
        return pbfBlock.Type switch
        {
            PBFBlockType.OSMHeader => new OSMHeaderBlock(
                Index: pbfBlock.Index,
                Data: HeaderBlock.Parser.ParseFrom(pbfBlock.Bytes)),
            PBFBlockType.OSMData => new OSMDataBlock(
                Index: pbfBlock.Index,
                Data: PrimitiveBlock.Parser.ParseFrom(pbfBlock.Bytes))
        };
    }

    public List<OSMNode> DecodeNodes(OSMDataBlock block) 
    {
        var nodeList = new List<OSMNode>();

        foreach (var group in block.Data.Primitivegroup)
        {
            if (group.Nodes.Count > 0)
            {
                nodeList.AddRange(this.DecodeOrdinaryNodes(block, group.Nodes));
            }

            if (group.Dense != null)
            {
                nodeList.AddRange(this.DecodeDenseNodes(block, group.Dense));
            }
        }

        return nodeList;
    }

    public List<OSMWay> DecodeWays(OSMDataBlock block)
    {
        var ways = new List<OSMWay>();

        foreach (var group in block.Data.Primitivegroup)
        {
            foreach (var way in group.Ways)
            {
                var tags = this.DecodeTags(block, way.Keys, way.Vals);

                var nodeIDs = new List<long>();
                var nodeID = 0L;

                // Way node references are stored as deltas from the previous reference
                foreach (var nodeIDDelta in way.Refs)
                {
                    nodeID += nodeIDDelta;
                    nodeIDs.Add(nodeID);
                }

                var newWay = new OSMWay(
                    ID: way.Id,
                    NodeIDs: nodeIDs,
                    Tags: tags);

                ways.Add(newWay);
            }
        }

        return ways;
    }

    private List<OSMNode> DecodeOrdinaryNodes(OSMDataBlock block, RepeatedField<Node> nodes)
    {
        var nodeList = new List<OSMNode>();

        foreach (var node in nodes)
        {
            var newNode = new OSMNode(
                ID: node.Id,
                Coordinates: this.DecodeCoordinates(block, node.Lat, node.Lon),
                Tags: this.DecodeTags(block, node.Keys, node.Vals));
            nodeList.Add(newNode);
        }

        return nodeList;
    }

    private List<OSMNode> DecodeDenseNodes(OSMDataBlock block, DenseNodes nodes)
    {
        var nodeId = 0L;
        var nodeLat = 0L;
        var nodeLon = 0L;
        var tagIndex = 0L;

        var nodeList = new List<OSMNode>();

        foreach (var (idDelta, latDelta, lonDelta) in Enumerable.Zip(
            nodes.Id, 
            nodes.Lat, 
            nodes.Lon))
        {
            // Dense IDs and coordinates are stored as deltas from the previous node
            nodeId += idDelta;
            nodeLat += latDelta;
            nodeLon += lonDelta;

            var newNode = new OSMNode(
                ID: nodeId,
                Coordinates: this.DecodeCoordinates(block, nodeLat, nodeLon),
                Tags: this.DecodeDenseTags(block, nodes, ref tagIndex));
            nodeList.Add(newNode);
        }

        return nodeList;
    }

    private OSMTags DecodeTags(OSMDataBlock block, RepeatedField<uint> keys, RepeatedField<uint> vals) 
    {
        var tags = new OSMTags();

        foreach (var (keyIndex, valIndex) in Enumerable.Zip(keys, vals))
        {
            var key = block.Data.Stringtable.S[(int)keyIndex].ToStringUtf8();
            var val = block.Data.Stringtable.S[(int)valIndex].ToStringUtf8();
            tags.Add(key, val);
        }

        return tags;
    }

    private OSMTags DecodeDenseTags(OSMDataBlock block, DenseNodes nodes, ref long tagIndex)
    {
        var tags = new OSMTags();

        if (nodes.KeysVals.Count == 0)
        {
            return tags;
        }

        // A zero delimiter marks the end of the current node's tags
        while (nodes.KeysVals[(int)tagIndex] != 0)
        {
            var keyIndex = nodes.KeysVals[(int)tagIndex];
            var valIndex = nodes.KeysVals[(int)tagIndex + 1];
            var key = block.Data.Stringtable.S[keyIndex].ToStringUtf8();
            var val = block.Data.Stringtable.S[valIndex].ToStringUtf8();
            tags.Add(key, val);
            tagIndex += 2;
        }

        tagIndex++;

        return tags;
    }

    private OSMCoordinates DecodeCoordinates(OSMDataBlock block, long lat, long lon)
    {
        // OSM stores coordinates using offsets and granularity in nanodegrees
        return new OSMCoordinates(
            Latitude: (block.Data.LatOffset + block.Data.Granularity * lat) / 1000000000.0,
            Longitude: (block.Data.LonOffset + block.Data.Granularity * lon) / 1000000000.0);
    }
}
