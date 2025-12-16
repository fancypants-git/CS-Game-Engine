using BulletSharp;
using Engine.Attributes;
using Engine.Internals;
using OpenTK.Mathematics;

namespace Engine.Components;

[ComponentMeta("BoxCollider")]
public class BoxCollider : Collider
{
    public BoxCollider(Entity e) : base(e, new BoxShape(0.5f), false)
    {
    }
    
    public BoxCollider(Entity e, bool isKinematic) : base(e, new BoxShape(0.5f), isKinematic)
    {
    }
    
    public BoxCollider(Entity e, Vector3 size) : base(e,
        new BoxShape(new BulletSharp.Math.Vector3(size.X/2f, size.Y/2f, size.Z/2f)), false)
    {
    }
    
    public BoxCollider(Entity e, Vector3 size, bool isKinematic) : base(e,
        new BoxShape(new BulletSharp.Math.Vector3(size.X/2f, size.Y/2f, size.Z/2f)), isKinematic)
    {
    }
}
