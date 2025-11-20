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
    public Texture Texture { get; set; }
    public Color Color { get; set; } = Color.White;
    public Texture SpecularMap { get; set; }
    public float SpecularExponent { get; set; } = 1;
    public float Transparency { get; set; } = 1f; // how much light an object blocks [0, 1] with 0 being no and 1 being all light.
    

    private bool _isDisposed = false;

    public Material()
    {
    }

    public void Use()
    {
        Shader.Use();
    }

    public void Use(Camera camera, Matrix4 model)
    {
        Use();
        
        Shader.UniformMat4("model", false, model);
        Shader.UniformMat4("projection", false, camera.Projection);
        Shader.UniformMat4("view", false, camera.View);
        
        Shader.Uniform3fv("cameraPos", camera.Transform.Position);
        Shader.Uniform3fv("viewDirection", camera.Transform.Forwards);
        
        Shader.Uniform3f("material.color", Color.R / 255.0f, Color.G / 255.0f, Color.B / 255.0f);
        
        
        Shader.Uniform1i("material.texture", 0);
        Shader.Uniform1i("material.specularMap", 1);
        Texture.Use(0);
        SpecularMap.Use(1);
        
        Shader.Uniform1f("material.specularExponent", SpecularExponent);
        
        Shader.Uniform1f("material.transparency", Transparency);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        
        Shader.Dispose();
        Texture?.Dispose();
        
        _isDisposed = true;
    }
}