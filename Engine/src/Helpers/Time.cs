namespace Engine.Helpers;

public static class Time
{
    // WARNING: all Delta Time variables are delayed by 1 frame!!
    public static int DeltaTimeMilliseconds { get; internal set; } = 0;
    public static float DeltaTime => DeltaTimeMilliseconds * 0.001f;
    public static int FixedDeltaTimeMilliseconds { get; internal set; } = 0;
    public static float FixedDeltaTime => FixedDeltaTimeMilliseconds * 0.001f;

    public static long ElapsedTimeMilliseconds { get; internal set; } = 0L;
    public static float ElapsedTime => ElapsedTimeMilliseconds * 0.001f;

    public static float TimeScale { get; set; } = 1f;
}