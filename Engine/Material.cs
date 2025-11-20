using System.Drawing;
using OpenTK.Graphics.OpenGLES2;
using OpenTK.Mathematics;

namespace Engine;

/// <summary>
/// Material contains the Material data of a Mesh
/// </summary>
public struct Material : IDisposable
{
    public Shader Shader { get; set; }
    public Texture? Texture { get; set; }
    public Color Color { get; set; } // ambient color
    public Color DiffuseColor { get; set; }
    public Color SpecularColor { get; set; }
    public float SpecularExponent { get; set; }
    

    private bool _isDisposed = false;

    public Material()
    {
    }

    public void Use()
    {
        Shader.Use();
        
        if (Texture != null)
            Texture.Use();
        else
            GL.BindTexture(TextureTarget.Texture2d, 0);
    }

    public void Use(Camera camera, Matrix4 model)
    {
        Use();
        
        Shader.UniformMat4("model", false, model);
        Shader.UniformMat4("projection", false, camera.Projection);
        Shader.UniformMat4("view", false, camera.View);
        Shader.Uniform3fv("cameraPos", camera.Transform.Position);
        Shader.Uniform3fv("viewDirection", camera.Transform.Forwards);
        Shader.Uniform3f("color", Color.R / 255.0f, Color.G / 255.0f, Color.B / 255.0f);
        Shader.Uniform3f("diffuseColor", DiffuseColor.R / 255.0f, DiffuseColor.G / 255.0f, DiffuseColor.B / 255.0f);
        Shader.Uniform3f("specularColor", SpecularColor.R / 255.0f, SpecularColor.G / 255.0f, SpecularColor.B / 255.0f);
        Shader.Uniform1f("specularExponent", SpecularExponent);
        Shader.Uniform1i("useTexture", Texture != null ? 1 : 0);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        
        Shader.Dispose();
        Texture?.Dispose();
        
        _isDisposed = true;
    }
}