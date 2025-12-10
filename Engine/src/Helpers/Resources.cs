using Engine.Internals;
using Engine.Rendering;

namespace Engine.Helpers;

public static class Resources
{
    public static readonly string ResourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources");
    
    public static string GetPath(params string[] relativePath)
    {
        string fullPath = Path.Combine(ResourcesPath, Path.Combine(relativePath));

        if (!File.Exists(fullPath) && !Directory.Exists(fullPath))
            Debug.LogWarn("Path does not exist:", fullPath);

        return fullPath;
    }



    public static Mesh? GetMesh(string path, out Material[] materials)
    {
        materials = [];
        path = GetPath(path); // make sure 'path' is a global path

        if (!File.Exists(path))
        {
            Debug.LogError("Path to Object file does not exist:", path, "\n\tPath must be a valid path to a .obj or binary file.");
            return null;
        }

        return MeshLoader.LoadMesh(path, out materials);
    }
    
    public static Shader GetShader(string name)
    {
        var vertPath = GetPath("Shaders", name + ".vert");
        var fragPath = GetPath("Shaders", name + ".frag");
        
        return new Shader(vertPath, fragPath);
    }
}
