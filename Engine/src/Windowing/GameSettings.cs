using Engine.Helpers;
using Engine.Maths;

namespace Engine.Windowing;

public class GameSettings
{
    public static readonly int Update = 0;
    public static readonly int FixedUpdate = 1;
    
    public int PhysicsUpdate { get; set; } = FixedUpdate;
    public double FixedUpdateDelta { get; set; } = 0.01;
    public bool DoublePhysicsPrecision { get; set; } = false;
    public Vector3 Gravity { get; set; } = new Vector3(0, -9.81f, 0);
    public Dictionary<string, DefaultLayer> Layers = new()
    {
        { "Terrain", new(1 << 0, LayerManager.BroadPhaseNonMoving) },
        { "Player", new(1 << 1, LayerManager.BroadPhaseMoving) },
        { "Enemy", new(1 << 2, LayerManager.BroadPhaseMoving) },
        { "Object", new(1 << 3, LayerManager.BroadPhaseMoving) },
    };
    
    
    public GameSettings(GameSettings from)
    {
        FixedUpdateDelta = from.FixedUpdateDelta;
        Gravity = from.Gravity;
        PhysicsUpdate = from.PhysicsUpdate;
        DoublePhysicsPrecision = from.DoublePhysicsPrecision;
        Layers = from.Layers;
    }
    public GameSettings() {}
    
    public static readonly GameSettings Default = new();
}
