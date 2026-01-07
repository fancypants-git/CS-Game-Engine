using System.Runtime.CompilerServices;
using Engine.Debugging;
using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering;

public class VertexBufferObject : IDisposable, IRequireRenderContext
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

    public void Upload<T>(T[] data, BufferUsage usage) where T : unmanaged
    {
        if (!Initialize()) return;

        Use();
        var size = data.Length * Unsafe.SizeOf<T>();
        GL.BufferData(BufferTarget.ArrayBuffer, size, data, usage);
    }

    public void Use()
    {
        if (!Initialize()) return;
        GL.BindBuffer(BufferTarget.ArrayBuffer, Handle);
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

    ~VertexBufferObject()
    {
        if (_isDisposed) return;

        Debug.LogMemLeak(GetType().Name);
        Dispose(false);
    }
}
