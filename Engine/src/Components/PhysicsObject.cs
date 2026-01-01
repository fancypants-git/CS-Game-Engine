using Engine.Attributes;
using Engine.Physics;
using Engine.Windowing;
using Engine.Maths;
using JoltPhysicsSharp;

namespace Engine.Components;

/// <summary>
///    A Physics Object signs that this Entity participates in the Physics System <br/>
///    It contains the Body and Layer
///    and assigns all Colliders and optionally a Rigidbody to the Physics System
/// </summary>
[ComponentMeta("PhysicsObject")]
[DisallowMultiple]
public class PhysicsObject : Component
{
    public PhysicsObject(Entity e, Layer layer, bool isKinematic) : base(e)
    {
        Layer = layer;
        Body = null!;
        IsKinematic = isKinematic;
    }

    public PhysicsObject(Entity e, Layer layer) : base(e)
    {
        Layer = layer;
        Body = null!;
        IsKinematic = false;
    }

    public Body Body { get; private set; }
    public Layer Layer { get; private set; }
    public bool IsKinematic { get; private set; }
    public bool IsDynamic { get; private set; }

    public override void Load()
    {
        Rigidbody? rb = Entity.GetComponent<Rigidbody>(true);
        float density = 0f;
        if (rb != null)
        {
            IsKinematic = false;
            IsDynamic = true;
            density = rb.Density;
        }

        var compound = new StaticCompoundShapeSettings();

        foreach (var collider in Entity.GetComponents<Collider>(true))
        {
            collider.InitializeShape(density);
            compound.AddShape(collider.Offset, collider.RotationQuat, collider.Shape);
        }

        MotionType motion = IsDynamic ? MotionType.Dynamic
            : IsKinematic ? MotionType.Kinematic
            : MotionType.Static;

        var bodySettings = new BodyCreationSettings(compound, Transform.Position, Transform.Rotation.ToSystem(), motion, Layer)
        {
            Friction = 0.8f,
            Restitution = 0f,
            LinearDamping = 0.05f,
            AngularDamping = 0.2f,
            AllowSleeping = true,
        };

        Body = PhysicsHandler.BodyInterface.CreateBody(bodySettings);

        PhysicsHandler.BodyInterface.AddBody(Body.ID, Activation.Activate);
    }

    public override void FixedUpdate()
    {
        if (Game.GameSettings.PhysicsUpdate == GameSettings.FixedUpdate
            && IsKinematic)
        {
            PhysicsHandler.BodyInterface.SetPositionAndRotation(Body.ID, Transform.Position, Transform.Rotation.ToSystem(), Activation.DontActivate);
        }
    }
}