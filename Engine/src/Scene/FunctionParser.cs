using Engine.Maths;

namespace Engine.Scene;

/// <summary>
/// Provides the base class for a Function Parser
/// </summary>
public abstract class FunctionParser
{
    /// <summary>
    /// Parses a <see cref="ValueNode"/> and returns a value as object
    /// </summary>
    /// <param name="node">The node to be parsed</param>
    /// <param name="type">The type of the return value, can be null if return value is null</param>
    /// <returns>The parsed value</returns>
    public abstract object Parse(ValueNode node, out Type? type);
}

// DEFAULT PARSERS

/// <summary>
/// A default fallback parser that returns null
/// </summary>
public class DefaultFunctionParser : FunctionParser
{
    public override object Parse(ValueNode node, out Type? type)
    {
        type = null;
        return null!;
    }
}

/// <summary>
/// Parses a Vector3 object
/// <br/>
/// Syntax:<br/>
/// vec3()<br/>
/// vec3(s)<br/>
/// vec3(x, y, z)
/// </summary>
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