using Engine.Components;
using Engine.Rendering;

namespace Engine.Scene;

public struct SceneData(SceneMeta meta) : IDisposable
{
    public SceneMeta Meta { get; } = meta;
    
    private List<Entity> _entities = [];
    private List<IDrawable> _drawables = [];
    
    public Camera ActiveCamera;
    
    private bool _isDisposed = false;
    

    /// <summary>
    /// Adds data to this scene.
    /// Please note that calling this method does not permanently add this data. If this scene is reset, or re-initialized this extra data is not included.
    /// </summary>
    /// <param name="data">The data of the scene to add</param>
    public void AddData(SceneData data)
    {
        _entities.AddRange(data._entities);
        AddDrawables(data.GetDrawables());
    }

    public readonly void AddEntity(Entity e)
    {
        _entities.Add(e);
    }

    public readonly Entity[] GetEntities()
    {
        return _entities.ToArray();
    }

    public readonly void AddDrawable(IDrawable drawable)
    {
        _drawables.Add(drawable);
    }

    public readonly void AddDrawables(IDrawable[] drawables)
    {
        _drawables.AddRange(drawables);
    }

    public readonly IDrawable[] GetDrawables()
    {
        return _drawables.ToArray();
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;

        foreach (var entity in _entities)
            entity.Dispose();
        
        foreach (var drawable in _drawables)
            drawable.Dispose();
        
        _entities.Clear();
        _drawables.Clear();

        _isDisposed = true;
    }
}