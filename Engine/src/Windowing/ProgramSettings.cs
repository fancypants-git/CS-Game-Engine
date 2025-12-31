using Engine.Debugging;
using Engine.Maths;
using OpenTK.Windowing.Common;

namespace Engine;

public class ProgramSettings
{
    public ContextFlags Flags { get; set; } = ContextFlags.Default;
    public VSyncMode VSync { get; set; } = VSyncMode.Off;

    public Vector2 WindowSize { get; set; } = new(800, 600);
    public string Title { get; set; } = "OpenGL Engine GameWindow";
    public WindowState WindowState { get; set; } = WindowState.Normal;
    public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;
    
    public uint LogFilter { get; set; } = uint.MaxValue;

    
    public ProgramSettings(ProgramSettings from)
    {
        Flags = from.Flags;
        VSync = from.VSync;
        WindowSize = from.WindowSize;
        Title = from.Title;
        WindowState = from.WindowState;
        WindowBorder = from.WindowBorder;
        LogFilter = from.LogFilter;
    }
    public ProgramSettings() {}


    public static readonly ProgramSettings Default = new();
    
    public static readonly ProgramSettings Debug = new() {
        Flags = ContextFlags.Debug,
        WindowSize = new(800, 600),
        Title = "[ DEBUG ] OpenGL Engine GameWindow",
        WindowState = WindowState.Normal,
        WindowBorder = WindowBorder.Resizable,
        LogFilter = (uint)(LogType.Info | LogType.Debug | LogType.Warning | LogType.Error | LogType.Fatal | LogType.Stack),
    };

    public static readonly ProgramSettings Release = new()
    {
        WindowSize = new(800, 600),
        Title = "[ RELEASE ] OpenGL Engine GameWindow",
        WindowState = WindowState.Maximized,
        WindowBorder = WindowBorder.Resizable,
        LogFilter = 0
    };
}
