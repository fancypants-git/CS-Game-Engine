using BulletSharp.Math;
using BulletSharp;
using Engine.Internals;

namespace Engine.Components;

public class Collider : Component
{
    public CollisionShape CollisionShape;
    public RigidBody Body;
    public readonly bool IsKinematic;
    
    public Collider(Entity entity, CollisionShape shape, bool isKinematic) : base(entity)
    {
        IsKinematic = isKinematic;
        
        CollisionShape = shape;
        var motionState = new TransformMotionState(Transform);
        var rigidbodyInfo = new RigidBodyConstructionInfo(0, motionState, CollisionShape, Vector3.Zero);
        Body = new RigidBody(rigidbodyInfo);
        if (isKinematic)
        {
            Body.CollisionFlags |= CollisionFlags.KinematicObject;
            Body.ActivationState = ActivationState.DisableDeactivation;
        }
        Physics.PhysicsHandler.AddRigidBody(Body);
        rigidbodyInfo.Dispose();
    }

    public override void Load()
    {
        CollisionShape.LocalScaling = new Vector3(Transform.GlobalSize.X, Transform.GlobalSize.Y, Transform.GlobalSize.Z);
    }

    protected override void Dispose(bool disposing)
    {
        if (IsDisposed) return;
        
        if (disposing)
        {
            CollisionShape.Dispose();
            Body.Dispose();
        }
        
        IsDisposed = true;
    }
}
