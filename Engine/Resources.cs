namespace Engine;

public static class Resources
{
    public static string GetPath(string relativePath)
    {
        if (Path.Exists(relativePath)) return relativePath;
        
        return Path.Combine(Directory.GetCurrentDirectory(), "Resources", relativePath);
    }


    public static Mesh? LoadMesh(string path)
    {
        path = GetPath(path); // make sure 'path' is a global path

        if (!File.Exists(path))
        {
            Debug.LogError("Path to Object file does not exist:", path, "\n\tPath must be a valid path to a .obj or binary file.");
            return null;
        }

        throw new NotImplementedException("Mesh Loading from File is not yet implemented.");
    }
}