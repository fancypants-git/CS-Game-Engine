using Engine.Maths;

namespace Engine.Helpers;

public static class Winfo
{
    public static Vector2 WindowSize { get; internal set; }
    public static float AspectRatio => WindowSize.X / WindowSize.Y;
}
