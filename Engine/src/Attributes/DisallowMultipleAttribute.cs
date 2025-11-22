namespace Engine;

public class DisallowMultipleAttribute(bool overrideIfExists = false) : Attribute
{
    public bool OverrideIfExists { get; } = overrideIfExists;
}