using System.ComponentModel;
using JoltPhysicsSharp;

namespace Engine;

public static class LayerManager
{
    public static readonly BroadPhaseLayer BroadPhaseNonMoving = 1;
    public static readonly BroadPhaseLayer BroadPhaseMoving = 2;
    
    private static Dictionary<string, DefaultLayer> _definedLayers = [];
    public static Dictionary<string, DefaultLayer> DefinedLayers => _definedLayers; 
    
    public static int Count => DefinedLayers.Keys.Count;
    public static readonly int BroadLayerCount = 2;
    
    public static void Initialize(Dictionary<string, DefaultLayer> layers)
    {
        _definedLayers = layers;
    }
    
    public static DefaultLayer FromName(string name)
    {
        if (DefinedLayers.TryGetValue(name, out DefaultLayer value))
            return value;
        
        return new(0, 0);
    }

    public static Layer FromName(string name, uint include = uint.MaxValue, uint exclude = 0)
    {
        if (DefinedLayers.TryGetValue(name, out DefaultLayer value))
            return value.WithMask(include, exclude);

        return new(0);
    }
}

// the layers used to initialize the Physics System
// these layers tell the PhysicsSystem which ObjectLayers are paired with each BroadPhaseLayer (both from JoltPhysics)
public readonly struct DefaultLayer(uint value, BroadPhaseLayer broadPhase)
{
    public readonly ObjectLayer Value = value;
    public readonly BroadPhaseLayer BroadPhaseLayer = broadPhase; // the broadphase layer this layer is assigned with
    
    public Layer WithMask(uint include = uint.MaxValue, uint exclude = 0)
    {
        return new(Value, include, exclude);
    }
}

// these are the actual layers used by colliders,
// they tell the body of the collider what layer it is and what layers it collides with (or which explicitely not)
public struct Layer
{
    public Layer(DefaultLayer layer, uint include = uint.MaxValue, uint exclude = 0)
    {
        Value = layer.Value;
        Include = include;
        Exclude = exclude;
    }
    public Layer(uint layer, uint include = uint.MaxValue, uint exclude = 0)
    {
        Value = layer;
        Include = include;
        Exclude = exclude;
    }
    
    public readonly uint Value;
    public uint Include { get; set; }
    public uint Exclude { get; set; }
    
    public static implicit operator ObjectLayer(Layer layer)
        => ObjectLayerPairFilterMask.GetObjectLayer(layer.Value, layer.Include & ~layer.Exclude);

    public override string ToString()
    {
        return $"Layer({Value} includes {Include} excludes {Exclude})";
    }
    
}
