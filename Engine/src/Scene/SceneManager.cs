using Engine.Components;
using Engine.Helpers;
using Engine.Internals;
using Engine.Scene;

// ReSharper disable once CheckNamespace
namespace Engine;

public static class SceneManager
{
    public static SceneData CurrentScene { get; private set; }
    public static Camera ActiveCamera => CurrentScene.ActiveCamera;
    
    private static bool _isDisposed = false;


    /// <summary>
    /// Loads and sets the current scene from the .scene at PATH
    /// This overrides the current scene.
    /// </summary>
    /// <param name="path">The full path to the .scene file<br/>
    /// (HINT: use Resources.GetPath() to get the full path from the "Resources" folder)
    /// </param>
    public static void InitializeScene(string path)
    {
        if (_isDisposed) return;

        var sceneData = SceneLoader.LoadSceneData(path);
        CurrentScene = sceneData;
        CurrentScene.ActiveCamera.SetViewportSize(Winfo.WindowSize.X, Winfo.WindowSize.Y);
    }

    /// <summary>
    /// Loads and adds the scene from the .scene at PATH to the current scene.
    /// </summary>
    /// <param name="path">The full path to the .scene file<br/>
    /// (HINT: use Resources.GetPath() to get the full path from the "Resources" folder)
    /// </param>
    public static void LoadScene(string path)
    {
        if (_isDisposed) return;
        
        var sceneData = SceneLoader.LoadSceneData(path);
        CurrentScene.AddData(sceneData);
    }
    
    /// <summary>
    /// Resets the currently active scene, does not keep extra scene data added using either LoadScene or CurrentScene.AddScene().
    /// </summary>
    public static void ResetCurrentScene()
    {
        if (_isDisposed) return;

        InitializeScene(CurrentScene.Meta.Path);
    }

    /// <summary>
    /// Updates every Entity in the Current Scene
    /// </summary>
    public static void UpdateScene()
    {
        if (_isDisposed) return;

        foreach (var entity in CurrentScene.Entities)
        {
            entity.Update();
        }
    }

    /// <summary>
    /// Updates every Entity in the Current Scene in the FixedUpdate thread.<br>/
    /// All physics should be handled on this thread
    /// </summary>
    public static void FixedUpdateScene()
    {
        if (_isDisposed) return;

        foreach (var entity in CurrentScene.Entities)
        {
            entity.FixedUpdate();
        }
    }

    /// <summary>
    /// Renders the Current Scene from the POV of the Active Camera
    /// </summary>
    public static void RenderScene()
    {
        if (_isDisposed) return;

        ActiveCamera.Render(CurrentScene.Drawables);
    }
    
    
    /// <summary>
    /// Disposes the SceneManager (not just CurrentScene), should only be called when closing the window!<br/>
    /// To Dispose the CurrentScene call SceneManager.CurrentScene.Dispose(), but should generally not be called,
    /// since this does not replace the CurrentScene causing unwanted behaviour and errors
    /// </summary>
    public static void Dispose()
    {
        if (_isDisposed) return;
        
        CurrentScene.Dispose();
        _isDisposed = true;
    }
}
