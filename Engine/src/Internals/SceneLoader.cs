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


public static class SceneLoader
{
    public readonly static string NewestVersion = "0.0.1";
    public readonly static Dictionary<string, SceneVersion> SceneVersions;

    static SceneLoader()
    {
        var json = File.ReadAllText(Resources.GetPath("Internal/SceneVersions.json"));
        SceneVersions = JsonConvert.DeserializeObject<Dictionary<string, SceneVersion>>(json);
    }
    
    public static SceneData LoadSceneData(string path)
    {
        var source = File.ReadAllText(path);
        var header = Regex.Match(source, @"(?ms)^meta_start\s*\n(?<meta>.*?)^meta_end\b", RegexOptions.Multiline);
        if (!header.Success)
        {
            Debug.LogError("Could not register Scene Header in", path);
            return new SceneData();
        }

        var versionMatch = Regex.Match(header.Groups["meta"].Value, @"(?m)^version\s+(?<version>[^\s]+)");
        string version;
        if (!versionMatch.Success)
        {
            Debug.LogWarn("Could not register Scene Version in", path, ". Make sure to always include Version!");
            Debug.LogWarn("Defaulting to newest version.");
            version = NewestVersion;
        }
        else
            version = versionMatch.Groups["version"].Value;

        

        var data = ParseScene(path, version);
        
        return data;
    }

    public static SceneData ParseScene(string path, string version)
    {
        if (!SceneVersions.TryGetValue(version, out var rules))
        {
            Debug.LogError("Scene Version \"", version, "\" was not registered. Make sure to use a registered version!");
            Debug.LogWarn("Using to newest version.");
            version = NewestVersion;
        }

        var meta = new SceneMeta {
            Path = path,
            Version = version
        };
        var data = new SceneData {
            Meta = meta,
            Entities = [],
            Drawables = []
        };

        var activeCameraId = "";

        Entity currentEntity = null!;
        
        var source = File.ReadAllText(path);
        string[] lines = source.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;
            
            var match = Regex.Match(line, @"\A(?<command>\w+) \s* (?<rest>.*)", RegexOptions.IgnorePatternWhitespace);

            if (!match.Success)
            {
                Debug.LogError("Failed to decode line", i, "of scene", path);
                continue;
            }

            var command = match.Groups["command"].Value;

            var argumentsMatches = Regex.Matches(match.Groups["rest"].Value, """(?<arg> "[^"]*" | \w+\([^)]*\) | [^\s"()]+)""",
                RegexOptions.IgnorePatternWhitespace);
            
            var args = argumentsMatches.Select(m => m.Groups["arg"].Value).ToArray();

            bool isMetaDataBlock = false;

            switch (command)
            {
                // meta data stuff
                case "scene":
                    isMetaDataBlock = true;
                    break;
                case "init":
                    isMetaDataBlock = false;
                    break;
                case "name":
                    meta.Name = DecodeString(args[0]);
                    break;
                case "camera":
                    activeCameraId = DecodeString(args[0]);
                    break;
                
                // global scene data
                case "include":
                    var includeData = LoadSceneData(DecodeString(args[0]));
                    data.AddData(includeData);
                    break;
                case "entity":
                    currentEntity = new Entity(DecodeString(args[0]));
                    data.Entities.Add(currentEntity);
                    break;
                case "add":
                    var ctypeStr = DecodeString(args[0]);
                    if (!ComponentRegistry.GetComponentType(ctypeStr, out var ctype))
                    {
                        Debug.LogWarn("Unknown component type at line", i);
                        break;
                    }

                    var parentParameter = new Parameter(currentEntity, typeof(Entity));
                    List<Parameter> parameters = [parentParameter];
                    
                    parameters.AddRange(DecodeParameters(args[1..]));

                    Component c;
                    try
                    {
                        c = ComponentRegistry.Create(ctype, parameters);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to create Component of type", ctype, "in line", i, "\n\b", e);
                        break;
                    }

                    switch (c)
                    {
                        case IDrawable drawable:
                            data.Drawables.Add(drawable);
                            break;
                        case Camera camera when currentEntity.Id == activeCameraId:
                            data.ActiveCamera = camera;
                            break;
                        case Transform transform:
                            currentEntity.Transform = transform;
                            break;
                    }

                    currentEntity.AddComponent(c);
                    break;
            }
        }
        
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
    private static Parameter[] DecodeParameters(string[] values)
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