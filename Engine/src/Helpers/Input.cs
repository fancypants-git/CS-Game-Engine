using Engine.Maths;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Helpers;

public class Input(KeyboardState ks, MouseState ms)
{
    private KeyboardState _ks = ks;
    private MouseState _ms = ms;
    
    public Vector2 MousePosition => _ms.Position;
    public Vector2 MouseDelta => _ms.Delta;
    public float ScrollDeltaY => _ms.ScrollDelta.Y;
    public Vector2 ScrollDelta => _ms.ScrollDelta;

    
    public bool IsKeyDown(Keys key) => _ks.IsKeyDown(key);

    public bool IsKeyReleased(Keys key) => _ks.IsKeyReleased(key);

    public bool IsKeyPressed(Keys key) => _ks.IsKeyPressed(key);

    public bool IsAnyKeyPressed() => _ks.IsAnyKeyDown;

    
    public bool IsButtonDown(MouseButton button) => _ms.IsButtonDown(button);

    public bool IsButtonReleased(MouseButton button) => _ms.IsButtonReleased(button);

    public bool IsButtonPressed(MouseButton button) => _ms.IsButtonPressed(button);

    public bool IsAnyButtonPressed() => _ms.IsAnyButtonDown;
}
