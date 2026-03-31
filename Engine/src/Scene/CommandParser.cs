using Engine.Debugging;
using Engine.Internals;

namespace Engine.Scene;

public abstract class CommandParser
{
    public abstract void Parse(Entity e, CommandNode node);
}


public class DefaultCommandParser : CommandParser
{
    public override void Parse(Entity e, CommandNode node)
    {
    }
}

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

        List<Parameter> parameters
    }
}