namespace Engine.Windowing;

public class WindowID
{
    WindowID(uint value)
    {
        Value = value;
    }

    public uint Value { get; init; }

    private static uint _maxValue = 0;

    public static WindowID New() => new WindowID(_maxValue++);
}