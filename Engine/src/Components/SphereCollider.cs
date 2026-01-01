using Engine.Attributes;
using JoltPhysicsSharp;

namespace Engine.Components;

[ComponentMeta("SphereCollider")]
public class SphereCollider : Collider
{
    public SphereCollider(Entity e, float radius) : base(e)
    {
        Radius = radius;
    }

    public float Radius { get; private set; }

    public override Shape CreateShape(float density)
    {
        return new SphereShape(Radius) { Density = density };
    }
}