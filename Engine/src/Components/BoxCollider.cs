using Engine.Attributes;
using Engine.Maths;
using JoltPhysicsSharp;

namespace Engine.Components;

/// <summary>
/// A Box shaped collider.  
/// Extension of the Collider component.
/// </summary>
[ComponentMeta("BoxCollider")]
public class BoxCollider : Collider
{
    /// <summary>
    /// Creates a new BoxCollider
    /// </summary>
    /// <param name="e">The parent entity</param>
    /// <param name="size">The size of this collider in the x, y and z axes. This is relative to the parent's size</param>
    public BoxCollider(Entity e, Vector3 size) : base(e)
    {
        Size = size;
    }

    /// <summary>
    /// Creates a new BoxCollider
    /// </summary>
    /// <param name="e">The parent entity</param>
    /// <param name="size">The size of this collider relative to the entity's size</param>
    /// <param name="offset">The offset of this collider in the entity's local space (Currently Unused)</param>
    public BoxCollider(Entity e, Vector3 size, Vector3 offset) : base(e, offset, Vector3.Zero)
    {
        Size = size;
    }

    /// <summary>
    /// Creates a new BoxCollider
    /// </summary>
    /// <param name="e">The parent entity</param>
    /// <param name="size">The size of this collider in the x, y and z axes. This is relative to the entity's Size</param>
    /// <param name="offset">The offset of this collider in the entity's local space (Currently Unused)</param>
    /// <param name="rotation">The rotation of this collider in the entity's local space (Currently Unused)</param>
    public BoxCollider(Entity e, Vector3 size, Vector3 offset, Vector3 rotation) : base(e, offset, rotation)
    {
        Size = size;
    }

    /// <summary>
    /// The size of this collider relative to the entity's size
    /// </summary>
    public Vector3 Size { get; private set; }

    /// <summary>
    /// Creates a BoxShape from the collider's settings
    /// </summary>
    /// <param name="density">(Optional) density of the shape, this is set to 0 if the entity does not contain an instance of <see cref="Rigidbody"/></param>
    /// <returns>The created BoxShape</returns>
    public override Shape CreateShape(float density)
    {
        return new BoxShape(Size * Transform.Size * 0.5f) { Density = density };
    }
}