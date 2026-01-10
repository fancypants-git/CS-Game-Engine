namespace Engine;

public struct ApplicationSettings
{
    public bool RunHeadless = false;

    public ApplicationSettings() {}

    public ApplicationSettings(ApplicationSettings from)
    {
        RunHeadless = from.RunHeadless;
    }

    public static ApplicationSettings Default => new();
}