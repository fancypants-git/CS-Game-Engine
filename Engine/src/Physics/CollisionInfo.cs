using Engine.Components;
using OpenTK.Mathematics;

namespace Engine.Physics;

public struct CollisionInfo
{
    public CollisionInfo(bool collided, float depth, Vector3 normal)
    {
        Collided = collided;
        CollisionDepth = depth;
        CollisionNormal = normal;
    }
    
    public bool Collided { get; }
    public float CollisionDepth { get; }
    public Vector3 CollisionNormal { get; }
    
    public static CollisionInfo NoCollision => new(false, 0f, Vector3.Zero);
}
