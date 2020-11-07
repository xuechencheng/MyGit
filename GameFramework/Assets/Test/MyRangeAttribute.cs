using UnityEngine;

public class MyRangeAttribute : PropertyAttribute
{
    public readonly float min;
    public readonly float max;

    public MyRangeAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}