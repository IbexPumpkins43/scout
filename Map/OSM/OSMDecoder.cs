using System.Diagnostics;
using Google.Protobuf.Collections;
using OSMPBF;

namespace Scout.Map;

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
                Data: PrimitiveBlock.Parser.ParseFrom(pbfBlock.Bytes)),
            _ => throw new UnreachableException()
        };
    }

    public List<OSMNode> DecodeNodes(OSMDataBlock block) 
    {
        List<OSMNode> nodeList = new();

        foreach (PrimitiveGroup group in block.Data.Primitivegroup)
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
        List<OSMWay> ways = new();

        foreach (PrimitiveGroup group in block.Data.Primitivegroup)
        {
            foreach (Way way in group.Ways)
            {
                OSMTags tags = this.DecodeTags(block, way.Keys, way.Vals);

                List<long> nodeIds = new();
                long nodeId = 0;

                // Way node references are stored as deltas from the previous reference
                foreach (long nodeIdDelta in way.Refs)
                {
                    nodeId += nodeIdDelta;
                    nodeIds.Add(nodeId);
                }

                OSMWay newWay = new(
                    Id: way.Id,
                    NodeIds: nodeIds,
                    Tags: tags);

                ways.Add(newWay);
            }
        }

        return ways;
    }

    private List<OSMNode> DecodeOrdinaryNodes(OSMDataBlock block, RepeatedField<Node> nodes)
    {
        List<OSMNode> nodeList = new();

        foreach (Node node in nodes)
        {
            OSMNode newNode = new(
                Id: node.Id,
                Coordinates: this.DecodeCoordinates(block, node.Lat, node.Lon),
                Tags: this.DecodeTags(block, node.Keys, node.Vals));
            nodeList.Add(newNode);
        }

        return nodeList;
    }

    private List<OSMNode> DecodeDenseNodes(OSMDataBlock block, DenseNodes nodes)
    {
        long nodeId = 0;
        long nodeLat = 0;
        long nodeLon = 0;
        long tagIndex = 0;

        List<OSMNode> nodeList = new();

        foreach (var (idDelta, latDelta, lonDelta) in Enumerable.Zip(
            nodes.Id, 
            nodes.Lat, 
            nodes.Lon))
        {
            // Dense IDs and coordinates are stored as deltas from the previous node
            nodeId += idDelta;
            nodeLat += latDelta;
            nodeLon += lonDelta;

            OSMNode newNode = new(
                Id: nodeId,
                Coordinates: this.DecodeCoordinates(block, nodeLat, nodeLon),
                Tags: this.DecodeDenseTags(block, nodes, ref tagIndex));
            nodeList.Add(newNode);
        }

        return nodeList;
    }

    private OSMTags DecodeTags(
        OSMDataBlock block,
        RepeatedField<uint> keys, 
        RepeatedField<uint> vals) 
    {
        OSMTags tags = new();

        foreach (var (keyIndex, valIndex) in Enumerable.Zip(keys, vals))
        {
            string key = block.Data.Stringtable.S[(int)keyIndex].ToStringUtf8();
            string val = block.Data.Stringtable.S[(int)valIndex].ToStringUtf8();
            tags.Add(key, val);
        }

        return tags;
    }

    private OSMTags DecodeDenseTags(OSMDataBlock block, DenseNodes nodes, ref long tagIndex)
    {
        OSMTags tags = new();

        if (nodes.KeysVals.Count == 0)
        {
            return tags;
        }

        // A zero delimiter marks the end of the current node's tags
        while (nodes.KeysVals[(int)tagIndex] != 0)
        {
            int keyIndex = nodes.KeysVals[(int)tagIndex];
            int valIndex = nodes.KeysVals[(int)tagIndex + 1];
            string key = block.Data.Stringtable.S[keyIndex].ToStringUtf8();
            string val = block.Data.Stringtable.S[valIndex].ToStringUtf8();
            tags.Add(key, val);
            tagIndex += 2;
        }

        tagIndex++;

        return tags;
    }

    private OSMCoordinates DecodeCoordinates(OSMDataBlock block, long lat, long lon)
    {
        // OSM stores coordinates using offsets and granularity in nanodegrees
        return new(
            Latitude: (block.Data.LatOffset + block.Data.Granularity * lat) / 1000000000.0,
            Longitude: (block.Data.LonOffset + block.Data.Granularity * lon) / 1000000000.0);
    }
}
