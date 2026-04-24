namespace Engine.Scene;

/// <summary>
/// Provides the base class for a Block Parser
/// </summary>
public abstract class BlockParser
{
    /// <summary>
    /// Parses a <see cref="BlockNode"/> into an entity
    /// </summary>
    /// <param name="node">The node to parse</param>
    /// <returns>The Entity that was created from the BlockNode</returns>
    public abstract Entity Parse(BlockNode node);

    /// <inheritdoc cref="SceneLoader.GetCommandParser(string)"/>
    public static CommandParser GetCommandParser(string tag)
        => SceneLoader.GetCommandParser(tag);
}


// DEFAULT PARSERS

/// <summary>
/// The default Entity Parser.
/// <br/>
/// Syntax:<br/>
/// entity (name) {}
/// </summary>
public class EntityBlockParser : BlockParser
{
    public override Entity Parse(BlockNode node)
    {
        Entity e = new();

        if (node.Arguments.Count == 1)
            e.Name = node.Arguments[0].GetString();

        foreach (CommandNode cmdNode in node.Children)
        {
            GetCommandParser(cmdNode.Name).Parse(e, cmdNode);
        }

        return e;
    }
}