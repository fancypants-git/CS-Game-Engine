using System.Drawing;
using OpenTK.Graphics.OpenGLES2;
using OpenTK.Mathematics;

namespace Engine;

/// <summary>
/// Material contains the Material data of a Mesh
/// </summary>
public struct Material : IDisposable
{
    // Shader //
    public Shader Shader { get; set; }
    
    // Color //
    public Color Color { get; set; } = Color.White;
    public Color DiffuseColor { get; set; } = Color.White;
    public Color SpecularColor { get; set; } = Color.White;
    
    // Texture Maps //
    public Texture? Texture { get; set; } // if a lit shader is used this acts as the texture, else this is the AmbientMap
    public Texture? DiffuseMap { get; set; }
    public Texture? SpecularMap { get; set; }
    public Texture? SpecularPowerMap { get; set; }

    // Misc Lighting Details //
    public float SpecularPower { get; set; } = 0;
    public float Transparency { get; set; } = 1f; // inverted of how much light does this object let through
    

    private bool _isDisposed = false;

    public Material()
    {
    }

    public void Use()
    {
        Shader.Use();
        
        // Color //
        Shader.Uniform3f("material.color", Color.R / 255f, Color.G / 255f, Color.B / 255f);
        Shader.Uniform3f("material.diffuseColor", DiffuseColor.R / 255f, DiffuseColor.G / 255f, DiffuseColor.B / 255f);
        Shader.Uniform3f("material.specularColor", SpecularColor.R / 255f, SpecularColor.G / 255f, SpecularColor.B / 255f);
        
        // Texture Maps //
            // Ambient Map | Texture //
        Shader.Uniform1i("material.useAmbientMap", Texture != null ? 1 : 0);
        if (Texture != null)
        {
            Texture.Use(0);
            Shader.Uniform1i("material.ambientMap", 0);
        }
        
            // Diffuse Map //
        Shader.Uniform1i("material.useDiffuseMap", DiffuseMap != null ? 1 : 0);
        if (DiffuseMap != null)
        {
            DiffuseMap.Use(1);
            Shader.Uniform1i("material.diffuseMap", 1);
        }
        
            // Specular Map //
        Shader.Uniform1i("material.useSpecularMap", SpecularMap != null ? 1 : 0);
        if (SpecularMap != null)
        {
            SpecularMap.Use(2);
            Shader.Uniform1i("material.specularMap", 2);
        }
        
            // Specular Power Map //
        Shader.Uniform1i("material.useSpecularPowerMap", SpecularPowerMap != null ? 1 : 0);
        if (SpecularPowerMap != null)
        {
            SpecularPowerMap.Use(3);
            Shader.Uniform1i("material.specularPowerMap", 3);
        }
        
        // Misc Lighting Details //
        Shader.Uniform1f("material.specularPower", SpecularPower);
        Shader.Uniform1f("material.transparency", Transparency);
    }

    public void Use(Camera camera, Matrix4 model)
    {
        Use();
        
        Shader.UniformMat4("model", false, model);
        Shader.UniformMat4("projection", false, camera.Projection);
        Shader.UniformMat4("view", false, camera.View);
        
        Shader.Uniform3fv("cameraPos", camera.Transform.Position);
        Shader.Uniform3fv("viewDirection", camera.Transform.Forwards);
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        
        Shader.Dispose();
        Texture?.Dispose();
        
        _isDisposed = true;
    }
}