using Engine.Maths;
using Engine.Windowing;

namespace Engine.Rendering;

public struct RenderContext
{
    public Vector2 Size { get; set; }
    public float AspectRatio { get; set; }

    public WindowID WindowID { get; set; }
}