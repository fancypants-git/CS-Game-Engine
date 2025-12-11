
namespace Engine.Physics;

public struct CollisionInfo(bool collided)
{
    public bool Collided { get; } = collided;
}
