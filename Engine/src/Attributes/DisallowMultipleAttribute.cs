namespace Engine.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class DisallowMultipleAttribute(bool overrideIfExists = true) : Attribute
{
    public bool OverrideIfExists { get; } = overrideIfExists;
}