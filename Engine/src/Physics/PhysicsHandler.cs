using BulletSharp;
using BulletSharp.Math;
using OpenTK.Graphics.Vulkan;

namespace Engine.Physics;

public static class PhysicsHandler
{
    private static CollisionConfiguration _collisionConfiguration;
    private static CollisionDispatcher _dispatcher;
    private static DbvtBroadphase _broadphase;
    public static DiscreteDynamicsWorld World;
    public static bool IsInitialized { get; private set; } = false;
    public static bool IsDisposed { get; private set; } = false;
    
    public static void Initialize(OpenTK.Mathematics.Vector3 gravity)
    {
        if (IsInitialized) return;
        
        _collisionConfiguration = new DefaultCollisionConfiguration();
        _dispatcher = new CollisionDispatcher(_collisionConfiguration);
        
        _broadphase = new DbvtBroadphase();
        World = new DiscreteDynamicsWorld(_dispatcher, _broadphase, null, _collisionConfiguration);
        World.Gravity = new Vector3(gravity.X, gravity.Y, gravity.Z);
        
        var co = new CollisionObject();
        
        
        IsInitialized = true;
    }
    
    public static void AddRigidBody(RigidBody rb)
    {
        World.AddRigidBody(rb);
    }
    
    public static void Update(float dt)
    {
        World.StepSimulation(dt);
    }

    public static void Dispose()
    {
        if (IsDisposed) return;
        
        //remove/dispose constraints
        int i;
        for (i = World.NumConstraints - 1; i >= 0; i--)
        {
            TypedConstraint constraint = World.GetConstraint(i);
            World.RemoveConstraint(constraint);
            constraint.Dispose();
        }
        
        //remove the rigidbodies from the dynamics world and delete them
        for (i = World.NumCollisionObjects - 1; i >= 0; i--)
        {
            CollisionObject obj = World.CollisionObjectArray[i];
            RigidBody body = obj as RigidBody;
            if (body != null && body.MotionState != null)
            {
                body.MotionState.Dispose();
            }
            World.RemoveCollisionObject(obj);
            obj.Dispose();
        }
        
        World.Dispose();
        _broadphase.Dispose();
        _dispatcher?.Dispose();
        _collisionConfiguration.Dispose();
        
        IsDisposed = true;
    }
}
