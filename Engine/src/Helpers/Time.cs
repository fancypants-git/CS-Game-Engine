namespace Engine.Helpers;

public static class Time
{
    // WARNING: all Delta Time variables are delayed by 1 frame!!
    private static long _deltaTimeMilliseconds = 0L;
    private static long _fixedDeltaTimeMilliseconds = 0L;

    public static long DeltaTimeMilliseconds => _deltaTimeMilliseconds;
    public static float DeltaTime => _deltaTimeMilliseconds * 0.001f;

    public static long FixedDeltaTimeMilliseconds => _fixedDeltaTimeMilliseconds;
    public static float FixedDeltaTime => _fixedDeltaTimeMilliseconds * 0.001f;

    public static float TimeScale = 1f;

    public static void UpdateDeltaTime(long ms)
    {
        _deltaTimeMilliseconds = (long)(ms * TimeScale);
    }

    public static void UpdateFixedDeltaTime(long ms)
    {
        _fixedDeltaTimeMilliseconds = (long)(ms * TimeScale);
    }
}