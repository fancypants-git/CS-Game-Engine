using System.Runtime.CompilerServices;
using Engine.Debugging;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering;

public class VertexBufferObject : IGpuResource
{
    protected VertexBufferObject()
    {
        GpuResourceManager.Register(this);
    }

    private int _handle;
    public int Handle => _handle;


    /// <summary>
    /// Calls OpenTK to bind this buffer
    /// </summary>
    public void Use()
    {
        if (!IsInitialized) return;
        GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
    }

    /// <summary>
    /// Sends the data in data to the GPU to be stored in the buffer at Handle
    /// </summary>
    public void Upload<T>(T[] data, BufferUsage usage) where T : unmanaged
    {
        if (!IsInitialized) return;
        Use();
        int size = data.Length * Unsafe.SizeOf<T>();
        GL.BufferData(BufferTarget.ArrayBuffer, size, data, usage);
    }

    /// <summary>
    /// Create a new VertexBufferObject with data type T[]
    /// </summary>
    /// <returns>A new VertexBufferObject with data type T[]</returns>
    public static VertexBufferObject Create()
    {
        return new VertexBufferObject();
    }

    public bool IsInitialized { get; private set; }
    public bool IsDisposed { get; private set; }
    private RenderContext _context;
    public RenderContext Context => _context;

    public bool Initialize(RenderContext context)
    {
        if (IsInitialized) return true;

        _context = context;
        _handle = GL.GenBuffer();
        context.Register(this);

        IsInitialized = true;
        return true;
    }

    public void Dispose()
    {
        if (IsDisposed || !IsInitialized || !_context.IsAlive) return;

        GL.DeleteBuffer(_handle);
        GpuResourceManager.UnRegister(this);

        IsDisposed = true;
    }
    ~VertexBufferObject()
    {
        if (!IsDisposed && IsInitialized)
            Debug.LogMemLeak(GetType().Name);
    }
}
