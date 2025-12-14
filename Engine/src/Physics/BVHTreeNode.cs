using System.Drawing;
using Engine.Debugging;
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
    
    public void DrawNodeAndChildrenBounds(Matrix4 model, int maxDepth = -1, int depth = 0)
    {
        Color[] colors = [
            Color.White,
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Cyan,
            Color.Pink,
            Color.Purple
        ];
        if (maxDepth != -1 && depth > maxDepth) return;
        
        Debug.DrawBounds(Bounds.TransformedToModel(model), colors[depth]);
        ChildA?.DrawNodeAndChildrenBounds(model, maxDepth, depth+1);
        ChildB?.DrawNodeAndChildrenBounds(model, maxDepth, depth+1);
    }
    
    public void Split(int depth = 0, int maxDepth = 20, int minTris = 20)
    {
        if (depth == maxDepth || Triangles.Count <= minTris) return;
        
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
    
    public static CollisionInfo Collide(BVHTreeNode node0, Matrix4 model0,
                                        BVHTreeNode node1, Matrix4 model1)
    {
        // Precompute transform from node1 -> node0 local space
        Matrix4 node1ToNode0 = model1 * model0.Inverted();
        return Collide(node0, node1, node1ToNode0);
    }

    private static CollisionInfo Collide(BVHTreeNode node0, BVHTreeNode node1, Matrix4 node1ToNode0)
    {
        // Transform node1 bounds into node0's space
        Bounds transformedNode1Bounds = node1.Bounds.TransformedToModel(node1ToNode0);

        if (!Bounds.Collide(node0.Bounds, transformedNode1Bounds))
            return CollisionInfo.NoCollision;

        if (node0.IsLeafNode() && node1.IsLeafNode())
            return CollideLeafLeaf(node0, node1, node1ToNode0);

        if (node0.IsLeafNode())
            return CollideNodeLeaf(node1, node0, node1ToNode0);

        if (node1.IsLeafNode())
            return CollideNodeLeaf(node0, node1, node1ToNode0.Inverted());

        return CollideNodeNode(node0, node1, node1ToNode0);
    }

    public static CollisionInfo CollideNodeNode(BVHTreeNode node0, BVHTreeNode node1, Matrix4 node1ToNode0)
    {
        CollisionInfo best = CollisionInfo.NoCollision;

        CollisionInfo c;

        c = Collide(node0.ChildA, node1.ChildA, node1ToNode0);
        if (c.Collided && (!best.Collided || c.CollisionDepth < best.CollisionDepth))
            best = c;

        c = Collide(node0.ChildA, node1.ChildB, node1ToNode0);
        if (c.Collided && (!best.Collided || c.CollisionDepth < best.CollisionDepth))
            best = c;

        c = Collide(node0.ChildB, node1.ChildA, node1ToNode0);
        if (c.Collided && (!best.Collided || c.CollisionDepth < best.CollisionDepth))
            best = c;

        c = Collide(node0.ChildB, node1.ChildB, node1ToNode0);
        if (c.Collided && (!best.Collided || c.CollisionDepth < best.CollisionDepth))
            best = c;

        return best;
    }

    public static CollisionInfo CollideNodeLeaf(BVHTreeNode node, BVHTreeNode leaf, Matrix4 nodeToLeafSpace)
    {
        CollisionInfo collidedWithA = Collide(leaf, node.ChildA, nodeToLeafSpace.Inverted());
        CollisionInfo collidedWithB = Collide(leaf, node.ChildB, nodeToLeafSpace.Inverted());

        if (collidedWithA.Collided && collidedWithB.Collided)
            return collidedWithA.CollisionDepth < collidedWithB.CollisionDepth ? collidedWithA : collidedWithB;

        return collidedWithA.Collided ? collidedWithA : collidedWithB;
    }

    public static CollisionInfo CollideLeafLeaf(BVHTreeNode leaf0, BVHTreeNode leaf1, Matrix4 leaf1ToLeaf0)
    {
        CollisionInfo bestCollision = CollisionInfo.NoCollision;

        foreach (var t0 in leaf0.Triangles)
        {
            foreach (var t1 in leaf1.Triangles)
            {
                // Transform t1 into t0's space
                BVHTriangle t1Transformed = t1.Transformed(leaf1ToLeaf0);

                Vector3[] axes = new[]
                {
                    t0.Normal,
                    t1Transformed.Normal,
                    Vector3.Cross(t0.Vertex1 - t0.Vertex0, t1Transformed.Vertex1 - t1Transformed.Vertex0),
                    Vector3.Cross(t0.Vertex1 - t0.Vertex0, t1Transformed.Vertex2 - t1Transformed.Vertex1),
                    Vector3.Cross(t0.Vertex1 - t0.Vertex0, t1Transformed.Vertex0 - t1Transformed.Vertex2),
                    Vector3.Cross(t0.Vertex2 - t0.Vertex1, t1Transformed.Vertex1 - t1Transformed.Vertex0),
                    Vector3.Cross(t0.Vertex2 - t0.Vertex1, t1Transformed.Vertex2 - t1Transformed.Vertex1),
                    Vector3.Cross(t0.Vertex2 - t0.Vertex1, t1Transformed.Vertex0 - t1Transformed.Vertex2),
                    Vector3.Cross(t0.Vertex0 - t0.Vertex2, t1Transformed.Vertex1 - t1Transformed.Vertex0),
                    Vector3.Cross(t0.Vertex0 - t0.Vertex2, t1Transformed.Vertex2 - t1Transformed.Vertex1),
                    Vector3.Cross(t0.Vertex0 - t0.Vertex2, t1Transformed.Vertex0 - t1Transformed.Vertex2),
                };

                CollisionInfo collision = BVHTriangle.Overlap(t0, t1Transformed, axes);
                if (collision.Collided && (!bestCollision.Collided || collision.CollisionDepth < bestCollision.CollisionDepth))
                    bestCollision = collision;
            }
        }

        return bestCollision;
    }
}
