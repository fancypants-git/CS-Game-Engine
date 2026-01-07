using Engine.Debugging;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering;

public class ElementBufferObject : IDisposable, IRequireRenderContext
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
            _handle = GL.GenBuffer();
        }

        IsInitialized = true;
        return true;
    }
    
    public void Upload(uint[] data, BufferUsage usage)
    {
        if (!Initialize()) return;

        Use();
        GL.BufferData(BufferTarget.ElementArrayBuffer, data.Length * sizeof(uint), data, usage);
    }

    public void Use()
    {
        if (!Initialize()) return;
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, Handle);
    }


    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (IsInitialized)
        {
            GL.DeleteBuffer(Handle);
        }

        _isDisposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ElementBufferObject()
    {
        if (_isDisposed) return;
        
        Debug.LogMemLeak(GetType().Name);
        Dispose(false);
    }
}
