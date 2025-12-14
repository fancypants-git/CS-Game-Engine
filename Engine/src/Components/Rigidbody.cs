using Engine.Attributes;
using Engine.Helpers;
using Engine.Debugging;
using OpenTK.Mathematics;
using Engine.Physics;

namespace Engine.Components;

[ComponentMeta("Rigidbody")]
[DisallowMultiple]
public class Rigidbody : Component
{
    public float Mass;
    public float Gravity;
    public Vector3 Velocity;
    
    public Rigidbody(Entity entity, float mass) : base(entity)
    {
        Mass = mass;
        Gravity = -9.81f;
    }

    public Rigidbody(Entity entity, float mass, float gravity) : base(entity)
    {
        Mass = mass;
        Gravity = gravity;
    }

    public override void FixedUpdate()
    {
        base.Update();

        var halfGravity = Gravity * Time.FixedDeltaTime / 2;
        
        Velocity.Y += halfGravity;
        Transform.Translate(Velocity * Time.FixedDeltaTime);
        Velocity.Y += halfGravity;
        
        foreach (var collider in Entity.GetComponents<Collider>(false))
        {
            var collisions = collider.CollidesWithAny();
            Vector3 totalPush = Vector3.Zero;
            foreach (CollisionInfo collision in collisions)
            {
                totalPush += collision.CollisionNormal * collision.CollisionDepth;
            }

            Transform.Translate(totalPush);
        }
    }
}
