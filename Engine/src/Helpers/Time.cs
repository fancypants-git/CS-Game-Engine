namespace Engine.Helpers;

public static class Time
{
    // WARNING: all Delta Time variables are delayed by 1 frame!!
    public static float DeltaTime => (float)DeltaTimeDouble;
    public static double DeltaTimeDouble { get; internal set; } = 0;
    public static float RenderTime => (float)RenderTimeDouble;
    public static double RenderTimeDouble { get; internal set; } = 0;
    public static float FixedDeltaTime => (float)FixedDeltaTimeDouble;
    public static double FixedDeltaTimeDouble { get; internal set; } = 0;

    public static float TimeScale { get; set; } = 1f;
}