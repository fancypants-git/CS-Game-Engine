using Engine.Maths;

namespace Engine.Scene;

public abstract class FunctionParser
{
    public abstract object Parse(ValueNode node, out Type? type); // parse a function type ValueNode and return an object

    public static FunctionParser GetFunctionParser(string tag)
        => SceneLoader.GetFunctionParser(tag);
}

// DEFAULT PARSERS

public class DefaultFunctionParser : FunctionParser
{
    public override object Parse(ValueNode node, out Type? type)
    {
        type = null;
        return null!;
    }
}

public class Vec3FunctionParser : FunctionParser
{
    public override object Parse(ValueNode node, out Type? type)
    {
        var vec = node.Arguments.Count switch
        {
            1 => new Vector3(node.Arguments[0].ParseFloat()),
            3 => new Vector3(node.Arguments[0].ParseFloat(), node.Arguments[1].ParseFloat(), node.Arguments[2].ParseFloat()),
            _ => Vector3.Zero,
        };

        type = typeof(Vector3);
        return vec;
    }
}