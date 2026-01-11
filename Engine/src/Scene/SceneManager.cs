using Engine.Components;
using Engine.Debugging;
using Engine.Internals;
using Engine.Scene;
using Engine.Windowing;

namespace Engine;

public class SceneManager : IDisposable
{
    public SceneData ActiveScene;
    public Camera ActiveCamera => ActiveScene.ActiveCamera;
    public SceneData? RequestedScene;

    public bool IsInitialized;
    public bool IsDisposed;

    /// <summary>
    /// Initializes this SceneManager Instance
    /// </summary>
    /// <param name="initialScene">The name of the initial Scene</param>
    public void Initialize(string initialScene)
    {
        IsInitialized = true;
        ActiveScene = SceneLoader.LoadSceneData(initialScene);
        LoadActiveScene();
    }


    /// <summary>
    /// Requests a new Scene to be Loaded in the next frame.
    /// </summary>
    /// <param name="path">The path to the scene (any .scene file) to be loaded</param>
    public void SetActive(string path)
    {
        RequestedScene = SceneLoader.LoadSceneData(path);
    }

    /// <summary>
    /// Loads the Active Scene to be usable for the Application
    /// </summary>
    public void LoadActiveScene()
    {
        if (!IsInitialized) return;

        foreach (Entity e in ActiveScene.GetEntities())
        {
            e.Load();
        }
    }

    /// <summary>
    /// Sets and Loads the Active Scene to be usable for the Application
    /// if any new Scene was requested.
    /// </summary>
    public void LoadRequestedScene()
    {
        if (RequestedScene == null) return;
        
        ActiveScene = (SceneData)RequestedScene;
        LoadActiveScene();

        RequestedScene = null;
    }

    /// <summary>
    /// Calls Update on every entity in the Active Scene
    /// </summary>
    public void UpdateScene()
    {
        if (!IsInitialized) return;

        foreach (Entity e in ActiveScene.GetEntities())
        {
            e.Update();
        }
    }

    /// <summary>
    /// Calls FixedUpdate on every entity in the Active Scene
    /// </summary>
    public void FixedUpateScene()
    {
        if (!IsInitialized) return;

        foreach (Entity e in ActiveScene.GetEntities())
        {
            e.FixedUpdate();
        }
    }


    private void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        if (disposing)
        {
            ActiveScene.Dispose();
            RequestedScene?.Dispose();
        }

        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SceneManager()
    {
        if (IsDisposed) return;

        Debug.LogMemLeak(GetType().Name);
        Dispose(false);
    }
}
