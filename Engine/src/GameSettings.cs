using Engine.Helpers;

namespace Engine;

public struct GameSettings
{
    public GameSettings()
    {
    }

    public GameSettings(GameSettings from)
    {
        FixedUpdateInterval = from.FixedUpdateInterval;
        PhysicsUpdate = from.PhysicsUpdate;
    }

    public enum UpdateType
    {
        Update,
        FixedUpdate
    }

    public int FixedUpdateInterval = 100;
    public UpdateType PhysicsUpdate = UpdateType.FixedUpdate;
    public string InitialScene = Resources.GetPath("Scenes/Example.scene");

    public static GameSettings Default => new();
}