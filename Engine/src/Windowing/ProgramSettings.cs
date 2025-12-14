using Engine.Debugging;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Engine;

public class ProgramSettings
{
    public double FixedUpdateDelta { get; set; } = 0.01;
    
    public ContextFlags Flags { get; set; } = ContextFlags.Default;
    public VSyncMode VSync { get; set; } = VSyncMode.Off;

    public Vector2i WindowSize { get; set; } = (800, 600);
    public string Title { get; set; } = "OpenGL Engine GameWindow";
    public WindowState WindowState { get; set; } = WindowState.Normal;
    public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;

    public LogFilter[] LogFilter { get; set; } = [];

    
    public ProgramSettings(ProgramSettings from)
    {
        FixedUpdateDelta = from.FixedUpdateDelta;
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
        LogFilter = [Debugging.LogFilter.Debug, Debugging.LogFilter.Warning, Debugging.LogFilter.Error, Debugging.LogFilter.Fatal],
        Flags = ContextFlags.Debug,
        WindowSize = (800, 600),
        Title = "[ DEBUG ] OpenGL Engine GameWindow",
        WindowState = WindowState.Normal,
        WindowBorder = WindowBorder.Resizable
    };

    public static readonly ProgramSettings Release = new()
    {
        LogFilter = [Debugging.LogFilter.Nothing],
        WindowSize = (800, 600),
        Title = "[ RELEASE ] OpenGL Engine GameWindow",
        WindowState = WindowState.Maximized,
        WindowBorder = WindowBorder.Resizable
    };
}
