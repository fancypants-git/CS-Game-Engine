namespace Engine.Scene;

public abstract class BlockParser
{
    public abstract Entity Parse(BlockNode node);

    public static CommandParser GetCommandParser(string name)
        => SceneLoader.GetCommandParser(name);
}


// DEFAULT PARSERS

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