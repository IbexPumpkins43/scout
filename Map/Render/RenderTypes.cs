using Raylib_cs;

namespace Scout.Map;

internal readonly record struct BoundBox(
    float MinX,
    float MaxX,
    float MinY,
    float MaxY);