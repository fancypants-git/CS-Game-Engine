using Engine.Debugging;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering;

public class VertexArrayObject : IGpuResource
{
    VertexArrayObject()
    {
        GpuResourceManager.Register(this);
    }

    private int _handle;
    public int Handle => _handle;

    private List<VertexAttribPointer> _unsetPointers = [];

    public void Use()
    {
        if (!IsInitialized) return;
        GL.BindVertexArray(_handle);
    }


    public void AttribPointer(uint location, int size, VertexAttribPointerType type, bool normalized, int stride,
        int offset)
    {
        AttribPointer(new VertexAttribPointer(location, size, type, normalized, stride, offset));
    }
    public void AttribPointer(VertexAttribPointer p)
    {
        _unsetPointers.Add(p);
        SetAttribPointers();
    }
    private void SetAttribPointers()
    {
        if (!IsInitialized) return;

        foreach (VertexAttribPointer p in _unsetPointers)
        {
            Use();
            GL.VertexAttribPointer(p.Location, p.Size, p.Type, p.Normalized, p.Stride, p.Offset);
            GL.EnableVertexAttribArray(p.Location);
        }

        _unsetPointers.Clear();
    }




    public static VertexArrayObject Create()
    {
        return new VertexArrayObject();
    }

    public bool IsInitialized { get; private set; }
    public bool IsDisposed { get; private set; }
    private RenderContext _context;
    public RenderContext Context => _context;

    public bool Initialize(RenderContext context)
    {
        if (IsInitialized) return true;

        _context = context;
        _handle = GL.GenVertexArray();
        context.Register(this);
        
        IsInitialized = true;
        SetAttribPointers();
        return true;
    }

    public void Dispose()
    {
        if (IsDisposed || !IsInitialized || !_context.IsAlive) return;

        GL.DeleteVertexArray(_handle);
        GpuResourceManager.UnRegister(this);

        IsDisposed = true;
    }
    ~VertexArrayObject()
    {
        if (!IsDisposed && IsInitialized)
            Debug.LogMemLeak(GetType().Name);
    }
}
