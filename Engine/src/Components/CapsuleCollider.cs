using Engine.Attributes;
using Engine.Maths;
using JoltPhysicsSharp;

namespace Engine.Components;

[ComponentMeta("CapsuleCollider")]
public class CapsuleCollider : Collider
{
    public CapsuleCollider(Entity e, float height, float radius) : base(e)
    {
        Height = height;
        Radius = radius;
    }

    public CapsuleCollider(Entity e, float height, float radius, Vector3 offset) : base(e, offset, Vector3.Zero)
    {
        Height = height;
        Radius = radius;
    }

    public CapsuleCollider(Entity e, float height, float radius, Vector3 offset, Vector3 rotation) : base(e, offset, rotation)
    {
        Height = height;
        Radius = radius;
    }

    public float Height;
    public float Radius;

    public override Shape CreateShape(float density)
    {
        return new CapsuleShape(Height * Transform.Size.Y * 0.5f, Radius) { Density = density };
    }
}
