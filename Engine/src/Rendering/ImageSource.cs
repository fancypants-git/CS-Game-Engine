using Engine.Debugging;
using StbImageSharp;

namespace Engine.Rendering;

public struct ImageSource
{
    public byte[] Data;
    public int Width;
    public int Height;
    public bool Usable;

    public static ImageSource FromFile(string path, ColorComponents components)
    {
        ImageResult? image = null;

        try
        {
            StbImage.stbi_set_flip_vertically_on_load(1);
            image = ImageResult.FromStream(File.OpenRead(path), components);
        }
        catch (Exception e)
        {
            Debug.LogErr("Error Loading Image.");
            Debug.LogErr(e);
            return new() { Data = [], Width = 0, Height = 0, Usable = false };
        }

        return new() { Data = image.Data, Width = image.Width, Height = image.Height, Usable = true };
    }
}