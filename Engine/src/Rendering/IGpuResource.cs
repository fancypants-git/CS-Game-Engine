namespace Engine.Rendering;

/// <summary>
/// Provides the basic methods to properly manage a GPU resource
/// </summary>
public interface IGpuResource : IDisposable
{
    /// <summary>
    /// Whether this GPU resource is initialized to the GPU
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Whether this GPU resource is disposed
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// The Window Context (RenderContext) this GPU resource is assigned to
    /// </summary>
    RenderContext Context { get; }

    /// <summary>
    /// Tries to initialize this GPU resource
    /// </summary>
    /// <param name="context">The Window Context (RenderContext) this GPU resource should be initialized to</param>
    /// <returns>Whether the initialization was succesfull</returns>
    bool Initialize(RenderContext context);
}