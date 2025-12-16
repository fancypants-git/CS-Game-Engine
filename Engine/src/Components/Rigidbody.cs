using Engine.Attributes;
using Engine.Helpers;
using OpenTK.Mathematics;
using BulletSharp;

namespace Engine.Components;

[ComponentMeta("Rigidbody")]
[DisallowMultiple]
public class Rigidbody : Component
{
    public float Mass;
    
    public Rigidbody(Entity entity, float mass) : base(entity)
    {
        Mass = mass;
    }

    public override void Load()
    {
        var colliders = Entity.GetComponents<Collider>(true);
        foreach (var collider in colliders)
        {
            collider.Load();
            ApplyMassTo(collider);
        }
    }

    private void ApplyMassTo(Collider collider)
    {
        collider.CollisionShape.CalculateLocalInertia(Mass, out var inertia);
        collider.Body.SetMassProps(Mass, inertia);
        collider.Body.UpdateInertiaTensor();
        collider.Body.ActivationState = ActivationState.ActiveTag;
    }
}
