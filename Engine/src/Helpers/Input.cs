using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Helpers;

public static class Input
{
    private static KeyboardState _ks;
    private static MouseState _ms;
    
    public static Vector2 MousePosition => _ms.Position;
    public static Vector2 MouseDelta => _ms.Delta;
    public static float ScrollDeltaY => _ms.ScrollDelta.Y;
    public static Vector2 ScrollDelta => _ms.ScrollDelta;

    public static void Initialize(KeyboardState ks, MouseState ms)
    {
        _ks = ks;
        _ms = ms;
    }

    
    public static bool IsKeyDown(Keys key) => _ks.IsKeyDown(key);

    public static bool IsKeyReleased(Keys key) => _ks.IsKeyReleased(key);

    public static bool IsKeyPressed(Keys key) => _ks.IsKeyPressed(key);

    public static bool IsAnyKeyPressed() => _ks.IsAnyKeyDown;

    
    public static bool IsButtonDown(MouseButton button) => _ms.IsButtonDown(button);

    public static bool IsButtonReleased(MouseButton button) => _ms.IsButtonReleased(button);

    public static bool IsButtonPressed(MouseButton button) => _ms.IsButtonPressed(button);

    public static bool IsAnyButtonPressed() => _ms.IsAnyButtonDown;
}