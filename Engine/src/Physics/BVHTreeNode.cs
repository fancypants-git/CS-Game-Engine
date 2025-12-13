using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Engine.Physics;

public class BVHTreeNode 
{    public BVHTreeNode(Bounds bounds, List<BVHTriangle> triangles)
    {
        Bounds = bounds;
        Triangles = triangles;
    }
    
    public BVHTreeNode()
    {
    }
    
    public Bounds Bounds = new(Vector3.PositiveInfinity, Vector3.NegativeInfinity);
    public List<BVHTriangle> Triangles = [];
    public BVHTreeNode ChildA;
    public BVHTreeNode ChildB;
    
    public bool IsLeafNode()
    {
        return ChildA == null && ChildB == null;
    }
    
    public void Split(int depth = 0, int maxDepth = 5)
    {
        if (depth == maxDepth) return;
        
        Vector3 size = Bounds.Size;
        int splitAxis = size.X > Math.Max(size.Y, size.Z) ? 0 : size.Y > size.Z ? 1 : 2;
        float splitPos = Bounds.Centre[splitAxis];
        
        ChildA = new();
        ChildB = new();
        
        foreach (BVHTriangle triangle in Triangles)
        {
            bool inChildA = triangle.Centre[splitAxis] < splitPos;
            BVHTreeNode child = inChildA ? ChildA : ChildB;
            child.Triangles.Add(triangle);
            child.Bounds.GrowToInclude(triangle.Vertex0);
            child.Bounds.GrowToInclude(triangle.Vertex1);
            child.Bounds.GrowToInclude(triangle.Vertex2);
        }
        
        ChildA.Split(depth + 1, maxDepth);
        ChildB.Split(depth + 1, maxDepth);
    }
    
    public static CollisionInfo Collide(BVHTreeNode node0, BVHTreeNode node1)
    {
        if (!Bounds.Collide(node0.Bounds, node1.Bounds))
            return CollisionInfo.NoCollision;
            
        if (node0.IsLeafNode() && node1.IsLeafNode())
            return CollideLeafLeaf(node0, node1);
        
        if (node0.IsLeafNode())
            return CollideNodeLeaf(node1, node0);
        
        if (node1.IsLeafNode())
            return CollideNodeLeaf(node0, node1);
        
        return CollideNodeNode(node0, node1);
    }
    
    public static CollisionInfo CollideNodeNode(BVHTreeNode node0, BVHTreeNode node1)
    {
        CollisionInfo best = CollisionInfo.NoCollision;

        CollisionInfo c;

        c = Collide(node0.ChildA, node1.ChildA);
        if (c.Collided && (!best.Collided || c.CollisionDepth < best.CollisionDepth))
            best = c;

        c = Collide(node0.ChildA, node1.ChildB);
        if (c.Collided && (!best.Collided || c.CollisionDepth < best.CollisionDepth))
            best = c;

        c = Collide(node0.ChildB, node1.ChildA);
        if (c.Collided && (!best.Collided || c.CollisionDepth < best.CollisionDepth))
            best = c;

        c = Collide(node0.ChildB, node1.ChildB);
        if (c.Collided && (!best.Collided || c.CollisionDepth < best.CollisionDepth))
            best = c;

        return best;
    }
    
    public static CollisionInfo CollideNodeLeaf(BVHTreeNode node, BVHTreeNode leaf)
    {
        CollisionInfo collidedWithA = Collide(leaf, node.ChildA);
        CollisionInfo collidedWithB = Collide(leaf, node.ChildB);
        
        if (collidedWithA.Collided && collidedWithB.Collided)
            return collidedWithA.CollisionDepth < collidedWithB.CollisionDepth ? collidedWithA : collidedWithB;
        
        return collidedWithA.Collided ? collidedWithA : collidedWithB;
    }
    
    public static CollisionInfo CollideLeafLeaf(BVHTreeNode leaf0, BVHTreeNode leaf1)
    {
        // Triangle - Triangle intersection test
        // Just use simple OBB intersection
        CollisionInfo bestCollision = CollisionInfo.NoCollision;
        foreach (var t0 in leaf0.Triangles)
        {
            foreach (var t1 in leaf1.Triangles)
            {
                Vector3[] axes = [
                    t0.Normal,
                    t1.Normal,
                    Vector3.Cross(t0.Vertex1 - t0.Vertex0, t1.Vertex1 - t1.Vertex0),
                    Vector3.Cross(t0.Vertex1 - t0.Vertex0, t1.Vertex2 - t1.Vertex1),
                    Vector3.Cross(t0.Vertex1 - t0.Vertex0, t1.Vertex0 - t1.Vertex2),
                    Vector3.Cross(t0.Vertex2 - t0.Vertex1, t1.Vertex1 - t1.Vertex0),
                    Vector3.Cross(t0.Vertex2 - t0.Vertex1, t1.Vertex2 - t1.Vertex1),
                    Vector3.Cross(t0.Vertex2 - t0.Vertex1, t1.Vertex0 - t1.Vertex2),
                    Vector3.Cross(t0.Vertex0 - t0.Vertex2, t1.Vertex1 - t1.Vertex0),
                    Vector3.Cross(t0.Vertex0 - t0.Vertex2, t1.Vertex2 - t1.Vertex1),
                    Vector3.Cross(t0.Vertex0 - t0.Vertex2, t1.Vertex0 - t1.Vertex2),
                ];
                
                CollisionInfo collision = BVHTriangle.Overlap(t0, t1, axes);
                if (collision.Collided && (!bestCollision.Collided || collision.CollisionDepth < bestCollision.CollisionDepth))
                    bestCollision = collision;
            }
        }
        
        return bestCollision;
    }
}
