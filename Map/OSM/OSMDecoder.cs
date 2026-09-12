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
                Data: HeaderBlock.Parser.ParseFrom(pbfBlock.Bytes.Span)),
            PBFBlockType.OSMData => new OSMDataBlock(
                Index: pbfBlock.Index,
                Data: PrimitiveBlock.Parser.ParseFrom(pbfBlock.Bytes.Span)),
            _ => throw new UnreachableException()
        };
    }

    public IEnumerable<OSMNodeView> DecodeNodes(OSMDataBlock block)
    {
        foreach (PrimitiveGroup group in block.Data.Primitivegroup)
        {
            foreach (OSMNodeView node in this.DecodeOrdinaryNodes(block, group.Nodes))
            {
                yield return node;
            }

            if (group.Dense != null)
            {
                foreach (OSMNodeView node in this.DecodeDenseNodes(block, group.Dense))
                {
                    yield return node;
                }
            }
        }
    }

    public IEnumerable<OSMWayView> DecodeWays(OSMDataBlock block)
    {
        foreach (PrimitiveGroup group in block.Data.Primitivegroup)
        {
            foreach (Way way in group.Ways)
            {
                yield return new(
                    Id: way.Id,
                    Refs: way.Refs,
                    Tags: new(block, way.Keys, way.Vals));
            }
        }
    }

    public long[] DecodeNodeIds(RepeatedField<long> refs)
    {
        long[] nodeIds = new long[refs.Count];
        long nodeId = 0;

        for (int i = 0; i < refs.Count; i++)
        {
            nodeId += refs[i];
            nodeIds[i] = nodeId;
        }

        return nodeIds;
    }

    private IEnumerable<OSMNodeView> DecodeOrdinaryNodes(
        OSMDataBlock block,
        RepeatedField<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            yield return new(
                Id: node.Id,
                Coordinates: this.DecodeCoordinates(block, node.Lat, node.Lon),
                Tags: new(block: block, keys: node.Keys, vals: node.Vals));
        }
    }

    private IEnumerable<OSMNodeView> DecodeDenseNodes(OSMDataBlock block, DenseNodes nodes)
    {
        long nodeId = 0;
        long nodeLat = 0;
        long nodeLon = 0;
        int tagIndex = 0;

        for (int i = 0; i < nodes.Id.Count; i++)
        {
            // Dense IDs and coordinates are stored as deltas from the previous node
            nodeId += nodes.Id[i];
            nodeLat += nodes.Lat[i];
            nodeLon += nodes.Lon[i];

            yield return new(
                Id: nodeId,
                Coordinates: this.DecodeCoordinates(block, nodeLat, nodeLon),
                Tags: this.DecodeDenseTagView(block, nodes, ref tagIndex));
        }
    }

    private OSMTagView DecodeDenseTagView(OSMDataBlock block, DenseNodes nodes, ref int tagIndex)
    {
        if (nodes.KeysVals.Count == 0)
        {
            return default;
        }

        int startIndex = tagIndex;
        while (nodes.KeysVals[tagIndex] != 0)
        {
            tagIndex += 2;
        }

        int tagCount = (tagIndex++ - startIndex) / 2;
        if (tagCount == 0)
        {
            return default;
        }

        return new(block: block, nodes: nodes, start: startIndex, count: tagCount);
    }

    private OSMCoordinates DecodeCoordinates(OSMDataBlock block, long lat, long lon)
    {
        // OSM stores coordinates using offsets and granularity in nanodegrees
        return new(
            Latitude: (block.Data.LatOffset + block.Data.Granularity * lat) / 1000000000.0,
            Longitude: (block.Data.LonOffset + block.Data.Granularity * lon) / 1000000000.0);
    }
}
