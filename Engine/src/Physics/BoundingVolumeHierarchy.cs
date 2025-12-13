using Engine.Components;
using Engine.Rendering;
using OpenTK.Mathematics;

namespace Engine.Physics;

public class BVH
{
    public BVH(Vector3[] vertices, uint[] triangleIndices, bool normalized, Matrix4 model)
    {
        Bounds bounds = new(Vector3.PositiveInfinity, Vector3.NegativeInfinity);
        
        // create the bounding box
        foreach (Vector3 vertex in vertices)
        {
            // first make the position in World Space if it was normalized to OpenGL coordinates (-1 to 1)
            if (normalized)
                bounds.GrowToInclude((model * new Vector4(vertex, 1)).Xyz);
            else
                bounds.GrowToInclude(vertex);
        }
        
        // create the BVH triangles
        BVHTriangle[] triangles = new BVHTriangle[triangleIndices.Length / 3];
        
        for (int i = 0; i < triangleIndices.Length; i++)
        {
            Vector3 vertex0 = vertices[i + 0];
            Vector3 vertex1 = vertices[i + 1];
            Vector3 vertex2 = vertices[i + 2];
            triangles[i] = new BVHTriangle(vertex0, vertex1, vertex2);
        }
        
        // construct the root node containing the entire collision mesh and then split it in half recursively
        Root = new(bounds, new(triangles));
        Root.Split();
    }
    
    public BVHTreeNode Root;
}
