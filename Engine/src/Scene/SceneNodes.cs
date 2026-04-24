using Engine.Internals;
using Engine.Debugging;

namespace Engine.Scene;

public class SceneNode
{
    public BlockNode MetaNode;
    public List<BlockNode> BlockNodes = new();
}

public class BlockNode
{
    public string Name;
    public List<ValueNode> Arguments = new();
    public List<CommandNode> Children = new();
}

public class CommandNode
{
    public string Name;
    public List<ValueNode> Arguments = new();
}

public class ValueNode
{
    public enum ValueType
    {
        Identifier,
        String,
        Number,
        Boolean,
        Function
    }
    public ValueType Type;
    public required string Value;
    public List<ValueNode> Arguments = new(); // Only used by Function Values to store the arguments of the function


    public int ParseInt()
    {
        if (int.TryParse(Value, out int result))
            return result;

        Debug.LogErr($"Failed to parse {Value} (ValueType: {Type}) as int.");
        return 0;
    }

    public float ParseFloat()
    {
        if (float.TryParse(Value, out float result))
            return result;

        Debug.LogErr($"Failed to parse {Value} (ValueType: {Type}) as float.");
        return 0;
    }

    public bool ParseBool()
    {
        return Value == "true";
    }

    public string GetString() => Value;

    public Parameter Parse()
    {
        if (Type == ValueType.Identifier)
            return new Parameter(GetString(), typeof(string));
        if (Type == ValueType.String)
            return new Parameter(GetString(), typeof(string));
        if (Type == ValueType.Boolean)
            return new Parameter(ParseBool(), typeof(bool));
        if (Type == ValueType.Number)
        {
            if (int.TryParse(GetString(), out int i))
                return new Parameter(i, typeof(int));
            if (float.TryParse(GetString(), out float f))
                return new Parameter(f, typeof(float));
        }
        if (Type == ValueType.Function)
        {
            object funcValue = FunctionParser.GetFunctionParser(Value).Parse(this, out Type? type);
            if (type != null)
                return new Parameter(funcValue, type);
        }

        return new Parameter();
    }
}