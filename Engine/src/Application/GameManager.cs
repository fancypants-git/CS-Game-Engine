using Engine.Components;

namespace Engine;

public class GameManager
{
    public SceneManager SceneManager = new();
    public Camera ActiveCamera => SceneManager.ActiveScene.ActiveCamera;

    public GameSettings Settings = GameSettings.Default;

    public virtual void Start()
    {
        SceneManager.Initialize(Settings.InitialScene);
    }

    public virtual void Update()
    {
        SceneManager.Update();
    }

    public virtual void FixedUpdate()
    {
        SceneManager.FixedUpdate();
    }

    public virtual void Dispose()
    {
        SceneManager.Dispose();
    }
}