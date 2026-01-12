namespace Engine;

public struct ApplicationSettings
{
    public ApplicationSettings() {}

    public ApplicationSettings(ApplicationSettings _base)
    {
        RunHeadless = _base.RunHeadless;
        WindowSettings = _base.WindowSettings;
        LogFilter = _base.LogFilter;
    }


    public bool RunHeadless = false;
    public WindowSettings WindowSettings = WindowSettings.Default;

    public uint LogFilter = uint.MaxValue;
}