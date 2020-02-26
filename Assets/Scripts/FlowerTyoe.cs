using System.Collections.Generic;
using System;

[Serializable]
public struct FlowerType
{
    public int MinZone;
    public IntRange Resources;
    public IntRange Size;
    public List<string> Names;

    public FlowerType(int minZone, IntRange resources, IntRange size, List<string> names)
    {
        MinZone = minZone;
        Resources = resources;
        Size = size;
        Names = names;
    }
}

