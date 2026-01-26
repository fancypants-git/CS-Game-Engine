using Engine.Attributes;
using Engine.Maths;
using JoltPhysicsSharp;

namespace Engine.Components;

/// <summary>
/// A Capsule shaped collider.
/// Extension of the Collider component
/// </summary>
[ComponentMeta("CapsuleCollider")]
public class CapsuleCollider : Collider
{
    /// <summary>
    /// Initializes the settings for a CapsuleCollider
    /// </summary>
    /// <param name="e">The parent entity</param>
    /// <param name="height">The height of the capsule relative to the entity's height</param>
    /// <param name="radius">The radius of the capsule</param>
    public CapsuleCollider(Entity e, float height, float radius) : base(e)
    {
        Height = height;
        Radius = radius;
    }

    /// <summary>
    /// Initializes the settings for a CapsuleCollider
    /// </summary>
    /// <param name="e">The parent entity</param>
    /// <param name="height">The height of the capsule relative to the entity's height</param>
    /// <param name="radius">The radius of the capsule</param>
    /// <param name="offset">The offset of this collider in the entity's local space (Currently Unused)</param>
    public CapsuleCollider(Entity e, float height, float radius, Vector3 offset) : base(e, offset, Vector3.Zero)
    {
        Height = height;
        Radius = radius;
    }

    /// <summary>
    /// Initializes the settings for a CapsuleCollider
    /// </summary>
    /// <param name="e">The parent entity</param>
    /// <param name="height">The height of the capsule relative to the entity's height</param>
    /// <param name="radius">The radius of the capsule</param>
    /// <param name="offset">The offset of this collider in the entity's local space (Currently Unused)</param>
    /// <param name="rotation">The rotation of this collider in the entity's local space (Currently Unused)</param>
    public CapsuleCollider(Entity e, float height, float radius, Vector3 offset, Vector3 rotation) : base(e, offset, rotation)
    {
        Height = height;
        Radius = radius;
    }

    /// <summary>
    /// The height of this capsule relative to the entity's height
    /// </summary>
    public float Height;
    /// <summary>
    /// The radius of this capsule
    /// </summary>
    public float Radius;

    /// <summary>
    /// Creates a new CapsuleShape from the collider's settings
    /// </summary>
    /// <param name="density">(Optional) density of the shape, this is set to 0 if the entity does not contain an instance of <see cref="Rigidbody"/></param>
    /// <returns>The created CapsuleShape</returns>
    public override Shape CreateShape(float density)
    {
        return new CapsuleShape(Height * Transform.Size.Y * 0.5f, Radius) { Density = density };
    }
}
