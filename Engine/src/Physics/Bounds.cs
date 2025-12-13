
using OpenTK.Mathematics;

namespace Engine.Physics;

public struct Bounds
{
    public Bounds(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    public Vector3 Min = new(0);
    public Vector3 Max = new(0);
    
    public readonly Vector3 Size => Max - Min;
    public readonly Vector3 HalfSize => Size * 0.5f;
    
    public readonly Vector3 Centre => Min + HalfSize;
    
    public readonly float Left => Min.X;
    public readonly float Right => Max.X;
    public readonly float Bottom => Min.Y;
    public readonly float Top => Max.Y;
    public readonly float Back => Min.Z;
    public readonly float Front => Max.Z;
    
    public void GrowToInclude(Vector3 point)
    {
        Min = Vector3.ComponentMin(Min, point);
        Max = Vector3.ComponentMax(Max, point);
    }
    
    public static bool Collide(Bounds a, Bounds b)
    {
        return a.Min.X < b.Max.X && b.Min.X < a.Max.X &&
                        a.Min.Y < b.Max.Y && b.Min.Y < a.Max.Y &&
                        a.Min.Z < b.Max.Z && b.Min.Z < a.Max.Z;
    }
}
