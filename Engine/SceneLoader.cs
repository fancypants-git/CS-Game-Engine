namespace Engine;

public struct SceneData
{
    public string Name { get; set; }
    public string Path { get; set; }
    
    public List<Entity> Entities { get; set; }
    public List<IDrawable> Drawables { get; set; }
    public Camera ActiveCamera { get; set; }
}

// TODO implement the scene language
internal static class SceneLoader
{
    internal static SceneData LoadSceneData(string path)
    {
        // TODO implement loading the .scene files into SceneData
        throw new NotImplementedException();
    }
    
    
    internal static void WriteSceneData(SceneData scene)
    {
        // TODO implement writing SceneData into a .scene file
        throw new NotImplementedException();
    }
}