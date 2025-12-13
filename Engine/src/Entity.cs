using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using Engine.Attributes;
using Engine.Components;
using Engine.Helpers;
using OpenTK.Graphics.OpenGL;
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

    public virtual void Update()
    {
        if (!IsActive) return;
        
        foreach (var c in _components)
        {
            try
            {
                c.Update();
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if (!IsActive) return;
        
        foreach (var c in _components)
        {
            try
            {
                c.FixedUpdate();
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
    

    public T AddComponent<T>(T component) where T : Component
    {
        // check if it has the DisallowMultiple attribute
        var attrib = (DisallowMultipleAttribute[])component.GetType().GetCustomAttributes(typeof(DisallowMultipleAttribute), true);
        if (attrib.Length == 1) // has a DisallowMultiple attribute
        {
            RemoveComponent(component.GetType());
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
    
    public bool GetComponent<T>(out T component, bool includeDisabled = false) where T : Component
    {
        foreach (var c in _components)
        {
            if (c is T Tc && (includeDisabled || c.Enabled))
            {
                component = Tc;
                return true;
            }
        }
        
        component = null;
        return false;
    }

    public Component[] GetComponents(bool includeDisabled = false)
    {
        if (includeDisabled)
            return _components.ToArray();
        
        return _components.Where(component => component.Enabled).ToArray();
    }
    
    public T[] GetComponents<T>(bool includeDisabled = false) where T : Component
    {
        if (includeDisabled)
            return _components.OfType<T>().ToArray();
            
        return _components.OfType<T>().Where(c => c.Enabled).ToArray();
    }

    public void RemoveComponent(Component component)
    {
        _components.Remove(component);
        component.Dispose();
    }
    public void RemoveComponent(Type type)
    {
        for (int i = 0; i < _components.Count; i++)
        {
            var c =_components[i];
            if (c.GetType() == type)
            {
                RemoveComponent(c);
                return;
            }
        }
    }


    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            foreach (var c in _components) c.Dispose();
        }
        
        _components.Clear();
        
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
