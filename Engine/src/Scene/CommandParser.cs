using Engine.Components;
using Engine.Debugging;
using Engine.Internals;

namespace Engine.Scene;

/// <summary>
/// Provides the base class for a Command Parser
/// </summary>
public abstract class CommandParser
{
    /// <summary>
    /// Parses a <see cref="CommandNode"/> and modifies an entity accordingly
    /// </summary>
    /// <param name="e">The entity to be modified</param>
    /// <param name="node">The node to be parsed</param>
    public abstract void Parse(Entity e, CommandNode node);
}

// DEFAULT PARSERS

/// <summary>
/// A default fallback parser that does nothing
/// </summary>
public class DefaultCommandParser : CommandParser
{
    public override void Parse(Entity e, CommandNode node)
    {
    }
}

/// <summary>
/// Adds a component to an Entity
/// <br/>
/// Syntax:<br/>
/// add [component] (...constructor arguments)
/// </summary>
public class AddCommandParser : CommandParser
{
    public override void Parse(Entity e, CommandNode node)
    {
        string componentName = node.Arguments[0].GetString();
        
        if (!ComponentRegistry.GetComponentType(componentName, out Type? componentType))
        {
            Debug.LogErr($"Could not find registered component {componentName}. Make sure to register the component.");
            return;
        }

        List<Parameter> parameters = [ new Parameter(e, typeof(Entity)) ];
        parameters.AddRange(node.Arguments[1..^0].Select(arg => arg.Parse()).ToList());

        if (!ComponentRegistry.Create(componentType, parameters, out Component? component))
        {
            Debug.LogErr($"Failed to create component {componentName}. Make sure the parameters match a constructor.");
            return;
        }

        e.AddComponent(component!);
    }
}