using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using OpenTK.Mathematics;

namespace Engine;

public struct SceneData
{
    public string Name { get; set; }
    public string Path { get; set; }

    public List<Entity> Entities { get; set; }
    public List<IDrawable> Drawables { get; set; }
    
    public Camera ActiveCamera { get; set; }
    public bool HasActiveCamera { get; set; }
}


internal static class SceneLoader
{
    internal static SceneData LoadSceneData(string path)
    {
        var data = new SceneData
        {
            Path = path,
            Entities = [],
            Drawables = [],
        };
        
        // global scene data
        var activeCameraId = "";
        
        // current entity data
        Entity currentEntity = null;
        
        

        var source = File.ReadAllText(path);
        Debug.LogInfo($"Loading scene {path}");

        var i = 0;
        foreach (var line in source.Split('\n'))
        {
            i++;
            var l = line.Trim();
            if (l.StartsWith('#') || string.IsNullOrWhiteSpace(l)) continue;
            
            // find the command and rest using RegEx
            const string commandPattern = @"\A(?<command>\w+) \s+ (?<rest>.*)";
            var match = Regex.Match(l, commandPattern, RegexOptions.IgnorePatternWhitespace);
            
            if (!match.Success)
            {
                Debug.LogError("Failed to decode line", i);
                continue;
            }
            
            var command = match.Groups["command"].Value;
            const string argPattern = """(?<arg>"[^"]*" | \w+\([^\)]*\) | [^\s"()<>]+)""";
            var matches = Regex.Matches(match.Groups["rest"].Value, argPattern, RegexOptions.IgnorePatternWhitespace);
            var args = matches.Select(m => m.Groups["arg"].Value).ToArray();

            switch (command)
            {
                // global scene data
                case "include":
                    var includeData = LoadSceneData(Resources.GetPath(DecodeString(args[0])));
                    if (includeData.HasActiveCamera)
                    {
                        if (data.HasActiveCamera)
                            Debug.LogWarn("Scene already defined ActiveCamera but was overridden by included scene in line", i);
                        
                        data.HasActiveCamera = true;
                        data.ActiveCamera = includeData.ActiveCamera;
                    }
                    
                    data.Entities.AddRange(includeData.Entities);
                    data.Drawables.AddRange(includeData.Drawables);
                    break;
                case "scene":
                    data.Name = DecodeString(args[0]);
                    break;
                case "camera":
                    data.HasActiveCamera = true;
                    activeCameraId = DecodeString(args[0]);
                    break;
                // entity scene data
                case "entity":
                    if (currentEntity != null)
                        data.Entities.Add(currentEntity);
                    
                    currentEntity = new Entity(DecodeString(args[0]));
                    break;
                case "add":
                    var name = DecodeString(args[0]);
                    if (!ComponentRegistry.GetComponentType(name, out var type))
                    {
                        Debug.LogError("Unknown component type at line", i);
                        continue;
                    }
                    
                    var parentParameter = new Parameter(currentEntity, typeof(Entity));
                    List<Parameter> parameters = [parentParameter];
                    
                    parameters.AddRange(DecodeParameters(args[1..]));

                    Component c;
                    try
                    {
                        c = ComponentRegistry.Create(type, parameters);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to create Component of type", name, "in line", i, "\n\b", e);
                        break;
                    }
                    

                    if (c is IDrawable drawable)
                        data.Drawables.Add(drawable);

                    if (type == typeof(Camera) && currentEntity.Id == activeCameraId)
                        data.ActiveCamera = (Camera)c;
                    
                    if (type == typeof(Transform))
                        currentEntity.Transform = (Transform)c;
                    
                    currentEntity.AddComponent(c);
                    break;
            }
        }
        
        if (currentEntity != null)
            data.Entities.Add(currentEntity);

        return data;
    }


    // --------------------------
    // Basic decoding helpers
    // --------------------------
    private static string DecodeString(string v)
    {
        return v.Trim().Trim('"');
    }

    private static Mesh DecodeMesh(string arg, out Material[] materials)
    {
        return (Mesh)Resources.GetMesh(DecodeString(arg), out materials);
    }

    private static Texture DecodeTexture(params string[] args)
    {
        string path = Resources.GetPath(DecodeString(args[0]));
        bool enabled = args.Length > 1 && bool.TryParse(args[1], out bool b) ? b : true;
        return new Texture(path, enabled);
    }

    private static Shader DecodeShader(params string[] args)
    {
        if (args.Length == 2 && !string.IsNullOrEmpty(args[1]))
            return new Shader(DecodeString(args[0]), DecodeString(args[1]));

        return Resources.GetShader(DecodeString(args[0]));
    }

    private static Vector3 DecodeVector3(string arg)
    {
        // Expecting: vec3(x), vec3(x, y, z)
        var match = Regex.Match(arg, @"vec3\(([^)]*)\)");
        if (!match.Success)
            throw new ArgumentException($"Invalid vec3 format: {arg}");

        var parts = match.Groups[1].Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => float.Parse(p.Trim()))
            .ToArray();

        return parts.Length switch
        {
            1 => new Vector3(parts[0]),
            3 => new Vector3(parts[0], parts[1], parts[2]),
            _ => throw new ArgumentException($"vec3 must have 1 or 3 values: {arg}")
        };
    }

    private static Vector2 DecodeVector2(string arg)
    {
        // Expecting: vec2(x), vec2(x, y)
        var match = Regex.Match(arg, @"vec2\(([^)]*)\)");
        if (!match.Success)
            throw new ArgumentException($"Invalid vec2 format: {arg}");

        var parts = match.Groups[1].Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => float.Parse(p.Trim()))
            .ToArray();

        return parts.Length switch
        {
            1 => new Vector2(parts[0]),
            2 => new Vector2(parts[0], parts[1]),
            _ => throw new ArgumentException($"vec3 must have 1 or 2 values: {arg}")
        };
    }

    // --------------------------
    // Main parameter decoder
    // --------------------------
    public static Parameter[] DecodeParameters(string[] values)
    {
        List<Parameter> parameters = [];
        
        foreach (var value in values)
        {
            var v = value.Trim();
            
            // Function-style argument: mesh(...), texture(...), shader(...), vec3(...)
            var match = Regex.Match(v, @"(?<name>\w+)\((?<args>[^\)]*)\)");

            // Boolean literal
            if (v.Equals("true", StringComparison.CurrentCultureIgnoreCase) || v.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                parameters.Add(new Parameter(v.Equals("true", StringComparison.CurrentCultureIgnoreCase), typeof(bool)));
            // Single literal
            else if (value.Contains('.') && float.TryParse(v, out float f))
                parameters.Add(new Parameter(f, typeof(float)));
            // Integer literal
            else if (int.TryParse(v, out int i))
                parameters.Add(new Parameter(i, typeof(int)));
            else if (match.Success)
            {
                string name = match.Groups["name"].Value;
                string[] args = match.Groups["args"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim()).ToArray();

                switch (name)
                {
                    case "mesh":
                        var mesh = DecodeMesh(args[0], out var materials);
                        parameters.Add(new Parameter(mesh, typeof(Mesh)));
                        parameters.Add(new Parameter(materials, typeof(Material[])));
                        break;
                    case "texture":
                        parameters.Add(new Parameter(DecodeTexture(args), typeof(Texture)));
                        break;
                    case "shader":
                        parameters.Add(new Parameter(DecodeShader(args), typeof(Shader)));
                        break;
                    case "vec3":
                        parameters.Add(new Parameter(DecodeVector3(v), typeof(Vector3)));
                        break;
                    case "vec2":
                        parameters.Add(new Parameter(DecodeVector2(v), typeof(Vector2)));
                        break;
                }
            }
            // String Literal
            else
                parameters.Add(new Parameter(DecodeString(v), typeof(string)));
        }
        
        return parameters.ToArray();
    }
}