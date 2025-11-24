namespace Engine.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ComponentMetaAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}