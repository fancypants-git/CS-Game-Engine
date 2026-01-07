using Engine.Debugging;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering;

public sealed class VertexArrayObject : IDisposable, IRequireRenderContext
{
    private int _handle;
    public int Handle => _handle;

    private bool _isDisposed = false;

    public bool IsInitialized { get; set; }

    public bool Initialize()
    {
        if (!IRequireRenderContext.RenderContextExists()) return false;

        if (!IsInitialized)
        {
            _handle = GL.GenVertexArray();
        }

        IsInitialized = true;
        return true;
    }

    public void Use()
    {
        if (!Initialize()) return;
        GL.BindVertexArray(Handle);
    }


    public void VertexAttribPointer(uint location, int size, VertexAttribPointerType type, bool normalized, int stride,
        int offset)
    {
        if (!Initialize()) return;

        Use();
        GL.VertexAttribPointer(location, size, type, normalized, stride, offset);
        GL.EnableVertexAttribArray(location);
    }


    private void Dispose(bool dispose)
    {
        if (_isDisposed || !IsInitialized) return;

        GL.BindVertexArray(0);
        GL.DeleteVertexArray(Handle);

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~VertexArrayObject()
    {
        if (_isDisposed) return;

        Debug.LogMemLeak(GetType().Name);
        Dispose(false);
    }
}
