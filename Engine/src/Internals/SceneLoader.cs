using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Engine.Components;
using Engine.Helpers;
using Engine.Interfaces;
using Engine.Rendering;
using Engine.Scene;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace Engine.Internals;

public struct BlockData
{
    public string Command { get; set; }
    public List<(string command, string[] args)> Block { get; set; }
    public bool IsSceneMetaBlock { get; set; }
    
    public readonly string[][] GetArgs(string command) => Block.Where(p => p.command == command).Select(p => p.args).ToArray();
}


// public struct ComponentData
// {
//     public string Type { get; set; }
//     public string[] Arguments { get; set; }
// }
//
// public struct EntityData
// {
//     public string Id { get; init; }
//     public List<ComponentData> Components { get; set; }
// }

public static class SceneLoader
{
    public readonly static string NewestVersion = "0.0.1";
    public readonly static Dictionary<string, SceneVersion> SceneVersions;

    static SceneLoader()
    {
        var json = File.ReadAllText(Resources.GetPath("Internal/SceneVersions.json"));
        SceneVersions = JsonConvert.DeserializeObject<Dictionary<string, SceneVersion>>(json) ?? [];
    }

    public static SceneData LoadSceneData(string path)
    {
        var source = File.ReadAllText(path);
        var header = Regex.Match(source, @"(?ms)^meta\s*\{(?<meta>.*)\s*\}", RegexOptions.Multiline);
        if (!header.Success)
        {
            Debug.LogError("Could not register Scene Header in", path);
            return new SceneData();
        }

        var versionMatch = Regex.Match(header.Groups["meta"].Value, @"(?m)version\s+(?<version>[^\s]+)", RegexOptions.Multiline);
        string version;
        if (!versionMatch.Success)
        {
            Debug.LogWarn("Could not register Scene Version in", path, ". Make sure to always include Version!");
            Debug.LogWarn("Defaulting to newest version.");
            version = NewestVersion;
        }
        else
            version = versionMatch.Groups["version"].Value;

        Debug.LogInfo("Using version", version, "for scene", path);

        var data = ParseScene(path, version);

        return data;
    }

    public static SceneData ParseScene(string path, string version)
    {
        if (!SceneVersions.TryGetValue(version, out var rules))
        {
            Debug.LogError("Scene Version \"", version,
                "\" was not registered. Make sure to use a registered version!");
            Debug.LogWarn("Using to newest scene version.");
            version = NewestVersion;
        }

        var source = File.ReadAllText(path);

        var blockMatches = Regex.Matches(source,
            @"(?<name>[A-Za-z_]\w*)\s*\{\s*(?<inner>(?:[^{}]|\{(?<c>)|\}(?<-c>))*(?(c)(?!)))\s*\}",
            RegexOptions.Multiline);

        BlockData[] blockDatas = blockMatches.Select(m => new BlockData
        {
            Command = m.Groups["name"].Value,
            Block = ParseBlockToDictionary(m.Groups["inner"].Value),
            IsSceneMetaBlock = m.Groups["name"].Value == "meta"
        }).ToArray();

        
        // Actually parse the blocks into Entities
        var meta = new SceneMeta
        {
            Path = path,
            Version = version
        };
        var data = new SceneData
        {
            Meta = meta,
            Entities = [],
            Drawables = []
        };


        string activeCameraId = "";
        
        foreach (var block in blockDatas)
        {
            if (block.IsSceneMetaBlock)
            {
                foreach (var line in block.Block)
                    switch (line.command)
                    {
                        case "name":
                            meta.Name = DecodeString(line.args[0]);
                            break;
                        case "camera":
                            activeCameraId = DecodeString(line.args[0]);
                            break;
                    }

                continue;
            }

            switch (block.Command)
            {
                case "entity":
                    var entity = ParseEntityFromBlockData(block, out var drawables);
                    if (entity.Id == activeCameraId)
                        data.ActiveCamera = entity.GetComponent<Camera>(false);
                    if (drawables.Count > 0)
                        data.Drawables.AddRange(drawables);
                    
                    data.Entities.Add(entity);
                    break;
                default:
                    Debug.LogWarn("Block type", block.Command, "is not recognised.");
                    break;
            }
        }
        
        return data;
    }
    
    
    // --------------------------
    // Block Data decoders
    // --------------------------
    private static List<(string command, string[] args)> ParseBlockToDictionary(string source)
    {
        var lines = Regex.Split(source, @"\n");

        List<(string command, string[] args)> commands = [];
        
        foreach (var line in lines)
        {
            var l = line.Trim();

            if (l.StartsWith('#') || string.IsNullOrWhiteSpace(l)) continue;

            var matches = Regex.Match(l, @"(?<command>\w+)\s+(?<args>.*)", RegexOptions.IgnorePatternWhitespace);
            var argsMatches = Regex.Matches(matches.Groups["args"].Value,
                """(?<arg> "[^"]" | \w+\([^\)]*\) | [^\"\(\)\s]+)""",
                RegexOptions.IgnorePatternWhitespace);

            var command = matches.Groups["command"].Value;
            commands.Add((command, argsMatches.Select(m => m.Groups["arg"].Value).ToArray()));
        }

        return commands;
    }


    private static Entity ParseEntityFromBlockData(BlockData block, out List<IDrawable> drawables)
    {
        drawables = [];
        
        var idArray = block.GetArgs("id");
        string entityId = idArray.Length != 0 ? idArray[0][0] : Guid.NewGuid().ToString();
        
        var entity = new Entity(entityId);

        foreach (var line in block.Block)
        {
            switch (line.command)
            {
                case "add":
                    var typeStr = DecodeString(line.args[0]);
                    if (!ComponentRegistry.GetComponentType(typeStr, out var type))
                    {
                        Debug.LogWarn("Component of type", typeStr, "is not registered. Make sure to include the ComponentMeta attribute!");
                        break;
                    }

                    List<Parameter> args = [new Parameter(entity, entity.GetType())];
                    args.AddRange(DecodeParameters(line.args[1..]));
                    if (!ComponentRegistry.Create(type, args, out var component))
                    {
                        Debug.LogError("Failed to create Component of type", typeStr);
                    }

                    entity.AddComponent(component);
                    if (typeof(IDrawable).IsAssignableFrom(type))
                        drawables.Add((IDrawable)component);
                        
                    else if (typeof(Transform).IsAssignableFrom(type))
                        entity.Transform = (Transform)component;
                    break;
            }
        }
        
        
        
        return entity;
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
        bool enabled = args.Length <= 1 || !bool.TryParse(args[1], out bool b) || b;
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
    private static Parameter[] DecodeParameters(string[] values)
    {
        List<Parameter> parameters = [];

        foreach (var value in values)
        {
            var v = value.Trim();

            // Function-style argument: mesh(...), texture(...), shader(...), vec3(...)
            var match = Regex.Match(v, @"(?<name>\w+)\((?<args>[^\)]*)\)");

            // Boolean literal
            if (v.Equals("true", StringComparison.CurrentCultureIgnoreCase) ||
                v.Equals("false", StringComparison.CurrentCultureIgnoreCase))
                parameters.Add(new Parameter(v.Equals("true", StringComparison.CurrentCultureIgnoreCase),
                    typeof(bool)));
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
