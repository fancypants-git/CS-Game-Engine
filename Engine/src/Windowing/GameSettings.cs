using Engine.Helpers;
using OpenTK.Mathematics;

namespace Engine.Windowing;

public class GameSettings
{
    public static readonly int Update = 0;
    public static readonly int FixedUpdate = 1;
    
    public int PhysicsUpdate = FixedUpdate;
    public double FixedUpdateDelta { get; set; } = 0.01;
    public Vector3 Gravity { get; set; } = new Vector3(0, -9.81f, 0);
    
    
    public GameSettings(GameSettings from)
    {
        FixedUpdateDelta = from.FixedUpdateDelta;
        Gravity = from.Gravity;
    }
    public GameSettings() {}
    
    public static readonly GameSettings Default = new();
}
