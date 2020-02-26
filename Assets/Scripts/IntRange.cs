using System;

[Serializable]
public struct IntRange
{
    public int min;
    public int max;

    public IntRange(int min, int max)
    {
        if(max < min) {
            max = min + 1;
        }
        this.min = min;
        this.max = max;
    }
}