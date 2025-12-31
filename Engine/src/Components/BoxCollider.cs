using Engine.Attributes;
using Engine.Maths;
using JoltPhysicsSharp;

namespace Engine.Components;

[ComponentMeta("BoxCollider")]
public class BoxCollider : Collider
{
    public BoxCollider(Entity e, Vector3 size) : base(e)
    {
        Size = size;
    }

    public BoxCollider(Entity e, Vector3 size, Vector3 offset) : base(e, offset, Vector3.Zero)
    {
        Size = size;
    }

    public BoxCollider(Entity e, Vector3 size, Vector3 offset, Vector3 rotation) : base(e, offset, rotation)
    {
        Size = size;
    }

    public Vector3 Size { get; private set; }

    public override Shape CreateShape(float density)
    {
        return new BoxShape(Size * Transform.Size * 0.5f) { Density = density };
    }
}