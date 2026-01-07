using Engine.Debugging;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace Engine.Rendering;

public class Texture : IDisposable, IRequireRenderContext
{
    private int _handle;
    public int Handle => _handle;
    
    private ImageSource _source;
    private bool _useMipmaps;

    public bool IsInitialized { get; set; }
    private bool _isDisposed = false;

    public Texture(string path, bool generateMipmaps)
    {
        _source = ImageSource.FromFile(path, ColorComponents.RedGreenBlueAlpha);
        _useMipmaps = generateMipmaps;
    }

    public bool Initialize()
    {
        if (!IRequireRenderContext.RenderContextExists()) return false;
        if (IsInitialized) return true;

        _handle = GL.GenTexture();
        Use();

        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba, _source.Width, _source.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, _source.Data);

        if (_useMipmaps)
        {
            GL.GenerateMipmap(TextureTarget.Texture2d);

            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        else
        {
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        GL.BindTexture(TextureTarget.Texture2d, 0);

        IsInitialized = true;
        return true;
    }

    public void Use(uint textureUnit = 0)
    {
        if (!Initialize()) return;
        GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
        GL.BindTexture(TextureTarget.Texture2d, Handle);
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing && IsInitialized)
        {
            GL.BindTexture(TextureTarget.Texture2d, 0);
            GL.DeleteTexture(Handle);
        }

        _isDisposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    ~Texture()
    {
        if (_isDisposed) return;
        
        Debug.LogMemLeak(GetType().Name);
        Dispose(false);
    }
}
