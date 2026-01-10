using Engine.Helpers;
using Engine.Maths;
using Engine.Physics;
using Engine.Rendering;
using Engine.Scene;
using OpenTK.Graphics.OpenGL;

namespace Engine;

public class Game
{
    public Game(GameSettings settings)
    {
        Settings = settings;
    }

    public Game()
    {
        Settings = GameSettings.Default;
    }

    public GameSettings Settings;

    public SceneManager SceneManager { get; set; } = new();
    public PhysicsManager PhysicsManager { get; set; } = new();

    public virtual void Load()
    {
        PhysicsManager.Initialize(false);
        SceneManager.Initialize(Settings.InitialScene);
    }

    public virtual void Update()
    {
        if (Settings.PhysicsUpdate == GameSettings.UpdateType.Update)
            PhysicsManager.Update(Time.DeltaTime);

        SceneManager.UpdateScene();
    }

    public virtual void FixedUpdate()
    {
        if (Settings.PhysicsUpdate == GameSettings.UpdateType.FixedUpdate)
            PhysicsManager.Update(Time.FixedDeltaTime);

        SceneManager.FixedUpateScene();
    }

    public virtual void Render()
    {
        SceneManager.ActiveCamera.Render(SceneManager.ActiveScene.Drawables.ToArray());
    }

    public virtual void Close()
    {
        PhysicsManager.Dispose();
        SceneManager.Dispose();
    }
}