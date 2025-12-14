using Engine.Helpers;
using Engine.Components;
using Engine.Rendering;
using OpenTK.Mathematics;

namespace Engine.Physics;

public class BVH
{
    public BVH(Vector3[] vertices, uint[] triangleIndices)
    {
        Bounds bounds = new(Vector3.PositiveInfinity, Vector3.NegativeInfinity);
        
        // create the bounding box
        foreach (Vector3 vertex in vertices)
        {
            bounds.GrowToInclude(vertex);
        }
        
        // create the BVH triangles
        BVHTriangle[] triangles = new BVHTriangle[triangleIndices.Length / 3];
        
        for (int i = 0; i < triangleIndices.Length / 3; i++)
        {
            Vector3 vertex0 = vertices[triangleIndices[i*3 + 0]];
            Vector3 vertex1 = vertices[triangleIndices[i*3 + 1]];
            Vector3 vertex2 = vertices[triangleIndices[i*3 + 2]];
            triangles[i] = new BVHTriangle(vertex0, vertex1, vertex2);
        }
        
        // construct the root node containing the entire collision mesh and then split it in half recursively
        Root = new(bounds, new(triangles));
        Root.Split();
    }
    
    public BVHTreeNode Root;
}
