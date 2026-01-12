using Engine.Components;
using Engine.Debugging;
using Engine.Rendering;

namespace Engine.Scene;

public struct SceneData(SceneMeta meta) : IDisposable
{
    public SceneMeta Meta { get; } = meta;

    private List<Entity> _entities = [];
    private List<IDrawable> _drawables = [];

    public Camera ActiveCamera = null!;

    private bool _isDisposed = false;


    /// <summary>
    /// Adds data to this scene.
    /// Please note that calling this method does not permanently add this data. If this scene is reset, or re-initialized this extra data is not included.
    /// </summary>
    /// <param name="data">The data of the scene to add</param>
    public readonly void AddData(SceneData data)
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

        Debug.Log("This is another test");

        _entities.Clear();
        _drawables.Clear();

        _isDisposed = true;
    }
}