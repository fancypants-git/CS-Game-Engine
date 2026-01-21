using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering;

public struct ShaderSource
{
    public string Source;
    public ShaderType Type;

    public static ShaderSource FromFile(string path, ShaderType type)
    {
        string source = File.ReadAllText(path);

        return new() { Source = source, Type = type };
    }
}