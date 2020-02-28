using System.Collections.Generic;
using System;

public enum FlowerType { 
    Common,
    Seasonal,
    Rare,
    Unique
}

[Serializable]
public struct FlowerData
{
    public int MinZone;
    public IntRange Resources;
    public IntRange Size;
    public List<string> Names;

    public FlowerData(int minZone, IntRange resources, IntRange size, List<string> names)
    {
        MinZone = minZone;
        Resources = resources;
        Size = size;
        Names = names;
    }
}

