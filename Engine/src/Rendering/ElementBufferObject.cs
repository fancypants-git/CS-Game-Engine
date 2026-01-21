using Engine.Debugging;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering;

public class ElementBufferObject : IGpuResource
{
    protected ElementBufferObject()
    {
        GpuResourceManager.Register(this);
    }

    private int _handle;
    public int Handle => _handle;

    public void Use()
    {
        if (!IsInitialized) return;
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
    }

    public void Upload(uint[] data, BufferUsage usage)
    {
        if (!IsInitialized) return;
        Use();
        GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, usage);
    }




    public static ElementBufferObject Create()
    {
        return new ElementBufferObject();
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

        IsDisposed = true;
    }
    ~ElementBufferObject()
    {
        if (!IsDisposed && IsInitialized)
            Debug.LogMemLeak(GetType().Name);
    }
}
