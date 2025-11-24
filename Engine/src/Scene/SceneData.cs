using Engine.Components;
using Engine.Interfaces;

namespace Engine.Scene;

public struct SceneData() : IDisposable
{
    public SceneMeta Meta { get; set; }
    
    public List<Entity> Entities { get; set; }
    public List<IDrawable> Drawables { get; set; }
    
    public Camera ActiveCamera { get; set; }
    
    private bool _isDisposed = false;
    

    /// <summary>
    /// Adds data to this scene.
    /// Please note that calling this method does not permanently add this data. If this scene is reset, or re-initialized this extra data is not included.
    /// </summary>
    /// <param name="data">The data of the scene to add</param>
    public void AddData(SceneData data)
    {
        Entities.AddRange(data.Entities);
        Drawables.AddRange(data.Drawables);
    }


    private void Dispose(bool disposing)
    {
        
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;

        foreach (var entity in Entities)
            entity.Dispose();
        
        foreach (var drawable in Drawables)
            drawable.Dispose();
        
        Entities.Clear();
        Drawables.Clear();

        _isDisposed = true;
    }
}