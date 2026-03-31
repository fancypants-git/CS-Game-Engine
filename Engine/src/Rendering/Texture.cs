using Engine.Debugging;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace Engine.Rendering;

public class Texture : IGpuResource
{
    protected Texture(ImageSource source, bool useMipmaps)
    {
        _source = source;
        _useMipmaps = useMipmaps;
        GpuResourceManager.Register(this);
    }

    private int _handle;
    public int Handle => _handle;

    private ImageSource _source;
    private bool _useMipmaps;

    public void Use(uint textureUnit = 0)
    {
        if (!IsInitialized) return;
        GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
        GL.BindTexture(TextureTarget.Texture2d, _handle);
    }




    public static Texture Create(string path, bool useMipmaps)
    {
        ImageSource source = ImageSource.FromFile(path, ColorComponents.RedGreenBlueAlpha);
        return new Texture(source, useMipmaps);
    }

    public bool IsInitialized { get; private set; }
    public bool IsDisposed { get; private set; }
    private RenderContext _context;
    public RenderContext Context => _context;

    public bool Initialize(RenderContext context)
    {
        if (IsInitialized) return true;
        if (!_source.Usable) return false;

        _context = context;

        _handle = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2d, _handle);

        GL.TexImage2D(TextureTarget.Texture2d, 0, InternalFormat.Rgba,
            _source.Width, _source.Height, 
            0, PixelFormat.Rgba, PixelType.UnsignedByte, _source.Data);

        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

        if (_useMipmaps)
        {
            GL.GenerateTextureMipmap(_handle);
            GL.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        }

        context.Register(this);
        IsInitialized = true;
        return true;
    }

    public void Dispose()
    {
        if (IsDisposed || !IsInitialized || !_context.IsAlive) return;

        GL.DeleteTexture(_handle);
        GpuResourceManager.UnRegister(this);

        IsDisposed = true;
    }
    ~Texture()
    {
        if (!IsDisposed && IsInitialized)
            Debug.LogMemLeak(GetType().Name);
    }
}
