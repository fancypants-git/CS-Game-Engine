namespace Engine;

public class DisallowMultipleAttribute : Attribute
{
    public bool OverrideIfExists { get; }

    public DisallowMultipleAttribute(bool overrideIfExists = false)
    {
        OverrideIfExists = overrideIfExists;
    }
}