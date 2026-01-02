using OpenTK.Windowing.Desktop;

namespace Engine.Windowing;

public struct WindowID
{
    public WindowID()
    {
        maxValue++;
        value = maxValue;
    }
    private static uint maxValue = 0;

    public uint value { get; }
}

public class Window : NativeWindow
{
    public WindowID ID;
}