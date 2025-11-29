using OpenTK.Mathematics;

namespace Engine.Helpers;

public static class Winfo
{
    public static Vector2i WindowSize { get; internal set; }
    public static float AspectRatio => (float)WindowSize.X / WindowSize.Y;
}