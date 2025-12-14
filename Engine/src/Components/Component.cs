using Engine.Helpers;

namespace Engine.Components;

public class Component(Entity entity) : IDisposable
{
    public bool Enabled = true;
    
    public Entity Entity = entity;
    public Transform Transform => entity.Transform;

    protected bool IsDisposed = false;

    public virtual void Load()
    {
    }
    
    public virtual void Update()
    {
    }
    public virtual void FixedUpdate()
    {
    }

    protected virtual void Dispose(bool disposing)
    {
        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~Component()
    {
        if (IsDisposed) return;
        Dispose(false);
    }
}
