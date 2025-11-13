namespace Engine;

public static class SceneManager
{
    public static Scene ActiveScene { get; private set; }
    public static Camera ActiveCamera => ActiveScene?.ActiveCamera;


    public static void SetMainCamera(Camera camera)
    {
        ActiveScene.ActiveCamera = camera;
    }

    public static void ActivateScene(Scene? scene, bool reset = true)
    {
        ActiveScene = scene;
        if (reset) scene?.Reset();
    }
}