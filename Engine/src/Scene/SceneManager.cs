using Engine.Debugging;
using Engine.Helpers;
// using Engine.Internals;
using Engine.Scene;

namespace Engine;

public class SceneManager : IDisposable
{
    private SceneData? _requestedScene;
    private SceneData _activeScene;
    public SceneData ActiveScene => _activeScene;

    public bool IsInitialized;
    public bool IsDisposed;


    public void Initialize(string initialScene)
    {
        IsInitialized = true;
        ForceLoadScene(initialScene);
    }

    public void LoadRequestedScene()
    {
        if (_requestedScene != null)
        {
            ForceSetActive((SceneData)_requestedScene);
            _requestedScene = null;
        }
    }

    public void Update()
    {
        if (!IsInitialized || IsDisposed) return;
        foreach (Entity e in _activeScene.GetEntities())
        {
            e.Update(); 
        }
    }

    public void FixedUpdate()
    {
        if (!IsInitialized || IsDisposed) return;

        foreach (Entity e in _activeScene.GetEntities())
        {
            e.FixedUpdate();
        }
    }

    public void Render()
    {
        if (!IsInitialized || IsDisposed) return;

        _activeScene.ActiveCamera.Render(_activeScene.GetDrawables());
    }


    public void SetActive(SceneData scene)
    {
        if (IsInitialized && !IsDisposed)
            _requestedScene = scene;
    }

    private void ForceSetActive(SceneData scene)
    {
        if (!IsInitialized || IsDisposed) return;

        try
        {
            _activeScene.Dispose();
        } catch (NullReferenceException) {}
        _activeScene = scene;

        foreach (Entity e in _activeScene.GetEntities())
        {
            e.Load();
        }
    }

    public void LoadScene(string path)
    {
        SetActive(SceneLoader.LoadScene(path));
    }

    private void ForceLoadScene(string path)
    {
        ForceSetActive(SceneLoader.LoadScene(path));
    }


    public static void DefaultSetActive(SceneData scene)
    {
        Application.Game.SceneManager.SetActive(scene);
    }

    public static void DefaultLoadScene(string path)
    {
        Application.Game.SceneManager.LoadScene(path);
    }


    private void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        if (disposing)
        {
            _activeScene.Dispose();
            _requestedScene?.Dispose();
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
