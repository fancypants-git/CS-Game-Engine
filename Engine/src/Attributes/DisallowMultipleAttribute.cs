namespace Engine.Attributes;

/// <summary>
/// Specifies that a component can only be assigned once to an entity instance
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class DisallowMultipleAttribute : Attribute
{
}