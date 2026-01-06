using Engine.Attributes;
using Engine.Debugging;
using Engine.Maths;
using Engine.Physics;
using Quaternion = System.Numerics.Quaternion;

namespace Engine.Components;

[ComponentMeta("Rigidbody")]
[DisallowMultiple]
public class Rigidbody : Component
{
    public Rigidbody(Entity e, float density) : base(e)
    {
        Density = density;
    }
    
    
    public float Density { get; private set; }

    public override void Load()
    {
        Application.Game.PhysicsManager.AddRigidBody(this);
    }
    
    public void SyncFromPhysics()
    {
        /**
         * get world (transposed) transform using BodyInterface.GetWorldTransform(BodyID)
         * get the Translation from the transform
         * get the Rotation Vector using QuaternionToAxis
         * set the Translation and Rotation Vector in the Transform component
         */
        if (!Entity.GetComponent(out PhysicsObject po, true))
        {
            Debug.LogWarn("Entity ", Entity.Id.ToString(), " does not contain a PhysicsObject, please consider adding this to the entity for participation in the physics system!");
            return;
        }

        if (!(Enabled && po!.Enabled)) return;

        Transform.Position = Application.Game.PhysicsManager.BodyInterface.GetPosition(po!.Body.ID);
        Transform.Rotation = Application.Game.PhysicsManager.BodyInterface.GetRotation(po!.Body.ID).ToOpenTK();
    }
}
