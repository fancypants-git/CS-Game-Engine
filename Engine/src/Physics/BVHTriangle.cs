using OpenTK.Mathematics;

namespace Engine.Physics;

public struct BVHTriangle
{
    public BVHTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
    {
        Vertex0 = vertex0;
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Centre = Vector3.Divide(Vertex0 + Vertex1 + Vertex2, 3);
        
        Vector3 edge0 = Vertex1 - Vertex0;
        Vector3 edge1 = Vertex2 - Vertex0;
        Normal = Vector3.Cross(edge0, edge1).Normalized();
    }
    
    public Vector3 Vertex0;
    public Vector3 Vertex1;
    public Vector3 Vertex2;
    public Vector3 Centre;
    public Vector3 Normal;
    
    public void ProjectTriangle(Vector3 axis, out float min, out float max)
    {
        float p0 = Vector3.Dot(Vertex0, axis);
        float p1 = Vector3.Dot(Vertex1, axis);
        float p2 = Vector3.Dot(Vertex2, axis);
        
        min = Math.Min(Math.Min(p0, p1), p2);
        max = Math.Max(Math.Max(p0, p1), p2);
    }
    
    public BVHTriangle Transformed(Matrix4 model)
    {
        return new BVHTriangle((new Vector4(Vertex0, 1) * model).Xyz,
                                (new Vector4(Vertex1, 1) * model).Xyz,
                                (new Vector4(Vertex2, 1) * model).Xyz);
    }
    
    public static bool Overlap(float minA, float maxA, float minB, float maxB, out float depth)
    {
        if (minA < maxB && minB < maxA)
        {
            depth = Math.Min(maxA - minB, maxB - minA);
            return true;
        }
        
        depth = 0;
        return false;
    }
    
    public static CollisionInfo Overlap(BVHTriangle t0, BVHTriangle t1, Vector3[] axes)
    {
        float minDepth = float.PositiveInfinity;
        Vector3 minAxis = Vector3.Zero;
        foreach (Vector3 axis in axes)
        {
            if (axis.LengthSquared < float.Epsilon) continue;
            
            axis.Normalize();
            
            t0.ProjectTriangle(axis, out float minA, out float maxA);
            t1.ProjectTriangle(axis, out float minB, out float maxB);
            
            if (!Overlap(minA, maxA, minB, maxB, out float depth))
                return CollisionInfo.NoCollision;
            
            if (depth < minDepth)
            {
                minDepth = depth;
                minAxis = axis;
                Vector3 delta = t1.Centre - t0.Centre;
                if (Vector3.Dot(minAxis, delta) < 0)
                    minAxis = -minAxis;
            }
        }
        
        return new CollisionInfo(true, minDepth, minAxis);
    }
}
