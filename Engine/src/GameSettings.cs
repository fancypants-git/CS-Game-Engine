using Engine.Helpers;
using Engine.Maths;

namespace Engine;

public struct GameSettings
{
    public enum UpdateType
    {
        Update,
        FixedUpdate
    }

    public UpdateType PhysicsUpdate = UpdateType.FixedUpdate;
    public long FixedUpdateInterval = 100L;
    public string InitialScene = Resources.GetPath("Scenes/Example.scene");
    public Dictionary<string, DefaultLayer> Layers = new()
    {
        { "Terrain", new(1 << 0, LayerManager.BroadPhaseNonMoving) },
        { "Player", new(1 << 1, LayerManager.BroadPhaseMoving) },
        { "Enemy", new(1 << 2, LayerManager.BroadPhaseMoving) },
        { "Object", new(1 << 3, LayerManager.BroadPhaseMoving) },
    };


    public GameSettings(GameSettings from)
    {
        FixedUpdateInterval = from.FixedUpdateInterval;
        PhysicsUpdate = from.PhysicsUpdate;
        Layers = from.Layers;
    }
    public GameSettings() { }

    public static readonly GameSettings Default = new();
}
