using System.Drawing;
using Engine.Attributes;
using OpenTK.Mathematics;

namespace Engine.Components;

[ComponentMeta("BoxCollider")]
public class BoxCollider : Component
{
    public Vector3 Size;
    public Vector3 Offset;
    
    public BoxCollider(Entity entity) : base(entity) {}
    
    public BoxCollider(Entity entity, Vector3 size, Vector3 offset) : base(entity)
    {
        Size = size;
        Offset = offset;
    }
}
