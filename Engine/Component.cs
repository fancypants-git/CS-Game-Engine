namespace Engine;


public class Component : IDisposable
{
    public Entity parent;
    public Transform transform { get; }

    protected bool _isDisposed;

    public Component(Entity parent)
    {
        this.parent = parent;
        transform = parent.transform;
    }
    
    

    public virtual void Load()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Unload()
    {
        parent.RemoveComponent(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Component()
    {
        if (_isDisposed) return;
        
        Debug.LogMemLeak("Component");
        Dispose(false);
    }
}