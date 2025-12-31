using JoltPhysicsSharp;
using Engine.Maths;
using Quaternion = System.Numerics.Quaternion;

namespace Engine.Components;

/// <summary>
/// A Collider represents a Collision Shape with the given settings,<br/>
/// Collider Itself is not a valid Collider as it does not have any associated shape,<br/>
/// Only Children of this class (e.g. BoxCollider, CapsuleCollider) are a valid Collider, this class is just the base.
/// Please keep this in mind.
/// </summary>
public class Collider : Component
{
    public Collider(Entity e) : base(e)
    {
        Offset = Vector3.Zero;
        Rotation = Vector3.Zero;
        const float deg2rad = MathF.PI / 180f;
        RotationQuat = Quaternion.CreateFromYawPitchRoll(deg2rad * Rotation.Y, deg2rad * Rotation.X, deg2rad * Rotation.Z);
    }

    public Collider(Entity e, Vector3 offset, Vector3 rotation) : base(e)
    {
        Offset = offset;
        Rotation = rotation;
        const float deg2rad = MathF.PI / 180f;
        RotationQuat = Quaternion.CreateFromYawPitchRoll(deg2rad * Rotation.Y, deg2rad * Rotation.X, deg2rad * Rotation.Z);
    }

    private Shape? _shape;
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

    public Vector3 Offset { get; protected init; }
    public Vector3 Rotation { get; protected init; }
    public Quaternion RotationQuat { get; protected init; }

    public virtual Shape CreateShape(float density)
        => null!;

    public virtual void InitializeShape(float density)
    {
        Shape = CreateShape(density);
    }
}
