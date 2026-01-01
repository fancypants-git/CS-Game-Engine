using Quaternion =  System.Numerics.Quaternion;
using Engine.Components;
using Engine.Debugging;
using JoltPhysicsSharp;

namespace Engine.Physics;

public static class PhysicsHandler
{
    public static bool IsInitialized { get; private set; } = false;
    public static bool IsDisposed { get; private set; } = false;
    
    public static PhysicsSystem PhysicsSystem { get; private set; }
    public static JobSystem JobSystem { get; private set; }
    public static BodyInterface BodyInterface { get; private set; }
    
    private static List<Rigidbody> _rigidbodies = [];

    private const int maxPhysicsSteps = 5;
    
    public static void Initialize(bool doublePrecision)
    {
        if (IsInitialized) return;
        
        // set the Trace and Failure handlers
        Foundation.SetTraceHandler((message) => 
        {
            Debug.LogInfo(message);
        });
        
        Foundation.SetAssertFailureHandler((inExpression, inMessage, inFile, inLine) =>
        {
            string message = inMessage ?? inExpression;
            string outMessage = $"JoltPhysics Assertion failure at {inFile}:{inLine}: {message}";
            Debug.LogErr(outMessage);
            
            throw new Exception(outMessage);
        });
        
        
        // initialize the foundation
        if (!Foundation.Init(false))
        {
            Debug.LogFatal("Failed to Initialize JoltPhysics Foundation!");
            return;
        }
        
        JobSystem = new JobSystemThreadPool();
        
        PhysicsSystemSettings settings = new();
        
        BroadPhaseLayerInterfaceMask broadPhaseLayerInterface = new((uint)LayerManager.BroadLayerCount);
        
        Dictionary<BroadPhaseLayer, uint> includeMasks = new();
        foreach (DefaultLayer layer in LayerManager.DefinedLayers.Values)
        {
            if (!includeMasks.TryGetValue(layer.BroadPhaseLayer, out uint mask))
                mask = 0;
                
            mask |= layer.Value;
            includeMasks[layer.BroadPhaseLayer] = mask;
        }
        
        foreach (var (layer, mask) in includeMasks)
            broadPhaseLayerInterface.ConfigureLayer(layer, mask, 0);
            
        settings.ObjectLayerPairFilter = new ObjectLayerPairFilterMask();
        settings.BroadPhaseLayerInterface = broadPhaseLayerInterface;
        settings.ObjectVsBroadPhaseLayerFilter = new ObjectVsBroadPhaseLayerFilterMask(broadPhaseLayerInterface);
        
        PhysicsSystem = new(settings);
        BodyInterface = PhysicsSystem.BodyInterface;
        
        IsInitialized = true;
    }
    
    public static void Update(float delta)
    {
        int stepCount = (int)Math.Ceiling(delta / (1/60f));
        stepCount = stepCount > maxPhysicsSteps ? maxPhysicsSteps : stepCount;
        var error = PhysicsSystem.Update(delta, stepCount, JobSystem);
        Debug.Assert(error == PhysicsUpdateError.None, $"PhysicsUpdate Failed due to {error}");
        
        foreach (var rb in _rigidbodies)
            rb.SyncFromPhysics();
    }
    
    public static void AddRigidBody(Rigidbody rb)
    {
        _rigidbodies.Add(rb);
    }
    
    public static void Dispose()
    {
        if (IsDisposed) return;
        
        PhysicsSystem.Dispose();
        
        IsDisposed = true;
    }
}
