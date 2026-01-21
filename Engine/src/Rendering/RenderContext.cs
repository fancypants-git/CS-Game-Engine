using Engine.Debugging;

namespace Engine.Rendering;

public class RenderContext : IDisposable
{
    private List<IGpuResource> _resources = [];
    public bool IsAlive = true;

    public void Register(IGpuResource resource)
    {
        _resources.Add(resource);
    }

    public void UnRegister(IGpuResource resource)
    {
        _resources.Remove(resource);
    }

    public void Dispose()
    {
        IsAlive = false;
        foreach (IGpuResource resource in _resources)
        {
            resource.Dispose();
        }

        _resources.Clear();
    }
}