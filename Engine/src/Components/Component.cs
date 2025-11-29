using System.Reflection;
using Engine.Helpers;

namespace Engine.Components;

public class Component(Entity entity) : IDisposable
{
    public bool Enabled = true;
    
    public Entity Entity = entity;
    public Transform Transform => entity.Transform;

    protected bool IsDisposed = false;


    public object this[string propertyName]
    {
        get
        {
            var info = GetType().GetProperty(propertyName);
            return info.GetValue(this, null);
        }
        set
        {              
            var myPropInfo = GetType().GetProperty(propertyName);
            myPropInfo.SetValue(this, value, null);
        }
    }
    

    public virtual void Load()
    {
    }
    
    public virtual void Update()
    {
    }
    public virtual void FixedUpdate()
    {
    }
    
    public virtual void Unload()
    {
        Entity.RemoveComponent(this);
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
        if (IsDisposed) return;
        
        Debug.LogMemLeak("Component");
        Dispose(false);
    }
}