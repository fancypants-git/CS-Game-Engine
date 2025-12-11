
using OpenTK.Mathematics;

namespace Engine.Mathematics;

public struct Bounds
{
    public Vector3 Min;
    public Vector3 Max;
    
    public readonly Vector3 Size => Max - Min;
    public readonly Vector3 HalfSize => Size / 2.0f;
    
    public readonly Vector3 Center => Min + HalfSize;
    
    public readonly float Left => Min.X;
    public readonly float Right => Max.X;
    public readonly float Bottom => Min.Y;
    public readonly float Top => Max.Y;
    public readonly float Back => Min.Z;
    public readonly float Front => Max.Z;
    
    public Bounds(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }
}
