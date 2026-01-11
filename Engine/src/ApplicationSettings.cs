namespace Engine;

public struct ApplicationSettings
{
    /// <summary>
    /// Whether the application is allowed to run without any windows opened,
    /// If false, closes the application when no windows are closed.
    /// </summary>
    public bool AllowHeadless = false;

    public ApplicationSettings() {}

    public ApplicationSettings(ApplicationSettings from)
    {
        AllowHeadless = from.AllowHeadless;
    }

    public static ApplicationSettings Default => new();
}