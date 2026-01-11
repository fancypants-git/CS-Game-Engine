using System.Collections;

namespace Engine.Windowing;

public class WindowID : IEquatable<WindowID>
{
    WindowID(uint value)
    {
        Value = value;
    }

    public uint Value { get; init; }

    private static uint _maxValue = 0;

    public static WindowID New() => new WindowID(_maxValue++);

    public bool Equals(WindowID? other)
    {
        return other != null && other.Value == Value;
    }
}