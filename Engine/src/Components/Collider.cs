using JoltPhysicsSharp;
using Engine.Maths;
using Quaternion = System.Numerics.Quaternion;

namespace Engine.Components;

/// <summary>
/// A Collider represents a Collision Shape with the given settings,<br/>
/// Collider Itself is not a valid Collider as it does not have any associated shape,<br/>
/// Only Children of this class (e.g. <see cref="BoxCollider"/>, <see cref="CapsuleCollider"/>) are a valid Collider, this class is just the base.
/// Please keep this in mind.
/// </summary>
public class Collider : Component
{
    /// <summary>
    /// CInitializes default collider settings without any shape
    /// </summary>
    /// <param name="e">The parent entity</param>
    public Collider(Entity e) : base(e)
    {
        Offset = Vector3.Zero;
        Rotation = Vector3.Zero;
        const float deg2rad = MathF.PI / 180f;
        RotationQuat = Quaternion.CreateFromYawPitchRoll(deg2rad * Rotation.Y, deg2rad * Rotation.X, deg2rad * Rotation.Z);
    }

    /// <summary>
    /// Initializes collider settings without any sahpe
    /// </summary>
    /// <param name="e">The parent entity</param>
    /// <param name="offset">The offset of this collider in the entity's local space</param>
    /// <param name="rotation">The rotation of this collider in the entity's local space</param>
    public Collider(Entity e, Vector3 offset, Vector3 rotation) : base(e)
    {
        Offset = offset;
        Rotation = rotation;
        const float deg2rad = MathF.PI / 180f;
        RotationQuat = Quaternion.CreateFromYawPitchRoll(deg2rad * Rotation.Y, deg2rad * Rotation.X, deg2rad * Rotation.Z);
    }

    /// <summary>
    /// The shape of this collider
    /// </summary>
    private Shape? _shape;
    /// <inheritdoc cref="_shape"/>
    public Shape Shape
    {
        get
        {
            return _shape!;
        }
        set
        {
            _shape ??= value;
        }
    }

    /// <summary>
    /// The offset of this collider in the entity's local space
    /// </summary>
    public Vector3 Offset { get; protected init; }
    /// <summary>
    /// The euler angles of this collider in the entity's local space
    /// </summary>
    public Vector3 Rotation { get; protected init; }
    /// <summary>
    /// The rotation of this collider in the entity's local space
    /// </summary>
    public Quaternion RotationQuat { get; protected init; }

    /// <summary>
    /// Method child colliders use to create their shape
    /// </summary>
    /// <param name="density">(Optional) density of the shape, this is set to 0 if the entity does not contain an instance of <see cref="Rigidbody"/></param>
    /// <returns>The shape that the collider created</returns>
    public virtual Shape CreateShape(float density)
        => null!;


    /// <summary>
    /// Creates and sets the shape of this collider
    /// </summary>
    /// <param name="density">(Optional) density of the shape, this is set to 0 if the entity does not contain an instance of <see cref="Rigidbody"/></param>
    public virtual void InitializeShape(float density)
    {
        Shape = CreateShape(density);
    }
}
