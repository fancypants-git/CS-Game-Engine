namespace Engine.Rendering;

public static class GpuResourceManager
{
    private static List<IGpuResource> _initializedResources = [];
    private static List<IGpuResource> _uninitializedResources = [];

    public static void Register(IGpuResource resource)
    {
        _uninitializedResources.Add(resource);
    }

    public static void UnRegister(IGpuResource resource)
    {
        _initializedResources.Remove(resource);
        _uninitializedResources.Remove(resource);
    }

    public static void InitializeResources(RenderContext context)
    {
        for (int i = _uninitializedResources.Count-1; i >= 0; i--)
        {
            IGpuResource resource = _uninitializedResources[i];
            if (resource.Initialize(context))
            {
                _uninitializedResources.RemoveAt(i);
                _initializedResources.Add(resource);
            }
        }
    }
}