using System.Reflection;
using OpenTK.Mathematics;

namespace Engine;

public class Entity : IDisposable
{
    public bool IsActive = true;

    public readonly string Id;

    public Transform Transform;

    protected List<Component> _components = [];

    protected bool _isDisposed = false;

    public Entity(string id)
    {
        Id = id;
    }


    public virtual void Load()
    {
        foreach (var c in _components)
            c.Load();
    }

    public virtual void EarlyUpdate()
    {
        if (!IsActive) return;

        Transform = GetComponent<Transform>(true);

        Component? currentComponent = null;
        try
        {
            foreach (var c in _components)
            {
                currentComponent = c;
                c.EarlyUpdate();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured when Updating Component", currentComponent, "|", e.Message, '\n', e);
        }
    }

    public virtual void Update()
    {
        if (!IsActive) return;

        Component? currentComponent = null;
        try
        {
            foreach (var c in _components)
            {
                currentComponent = c;
                c.Update();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error Occured when Updating Component", currentComponent, "|", e.Message, '\n', e);
        }
    }

    public virtual void Unload()
    {
        Component[] unloadComponents = new Component[_components.Count];
        _components.CopyTo(unloadComponents);
        foreach (var c in unloadComponents)
            c.Unload();
    }



    public T AddComponent<T>(T component) where T : Component
    {
        // check if it has the DisallowMultiple attribute
        var attrib = (DisallowMultipleAttribute[])component.GetType().GetCustomAttributes(typeof(DisallowMultipleAttribute), true);
        if (attrib.Length == 1) // has a DisallowMultiple attribute
        {
            if (attrib[0].OverrideIfExists)
            {
                RemoveComponent<T>();
            }
            else
            {
                var c = GetComponent<T>(true);
                if (c != null)
                    return c;
            }
        }
        
        _components.Add(component);

        return component;
    }

    public T? GetComponent<T>(bool includeDisabled = false) where T : Component
    {
        foreach (var component in _components)
        {
            if (component is T c && (includeDisabled || component.Enabled))
                return c;
        }

        return null;
    }

    public Component[] GetComponents(bool includeDisabled = false)
    {
        if (includeDisabled)
            return _components.ToArray();
        
        return _components.Where(component => component.Enabled).ToArray();
    }

    public void RemoveComponent(Component component)
    {
        _components.Remove(component);
    }
    
    public void RemoveComponent<T>() where T : Component
    {
        foreach (var component in _components)
        {
            if (component is T c)
                RemoveComponent(c);
        }
    }


    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            foreach (var c in _components) c.Dispose();
        }
        
        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Entity()
    {
        if (_isDisposed) return;
        
        Debug.LogMemLeak("Entity");
        Dispose(false);
    }
}