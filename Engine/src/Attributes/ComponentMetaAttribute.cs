namespace Engine.Attributes;

/// <summary>
/// Specifies the meta data of a component for use in scene files
/// </summary>
/// <param name="name">The name to use for the component</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ComponentMetaAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}