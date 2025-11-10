namespace Engine;

public static class Resources
{
    public static string GetPath(string relativePath)
    {
        if (Path.Exists(relativePath)) return relativePath;
        
        return Path.Combine(Directory.GetCurrentDirectory(), "Resources", relativePath);
    }
}