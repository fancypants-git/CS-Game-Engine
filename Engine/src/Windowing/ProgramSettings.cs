using Engine.Helpers;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Engine;

public class ProgramSettings
{
    public ContextFlags Flags { get; set; } = ContextFlags.Default;
    public VSyncMode VSync { get; set; } = VSyncMode.Off;

    public Vector2i WindowSize { get; set; } = (800, 600);
    public string Title { get; set; } = "OpenGL Engine GameWindow";
    public WindowState WindowState { get; set; } = WindowState.Normal;
    public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;
    
    public LogFilter[] LogFilter { get; set; } = [];

    
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
        WindowSize = (800, 600),
        Title = "[ DEBUG ] OpenGL Engine GameWindow",
        WindowState = WindowState.Normal,
        WindowBorder = WindowBorder.Resizable,
        LogFilter = [Helpers.LogFilter.Debug, Helpers.LogFilter.Warning, Helpers.LogFilter.Error, Helpers.LogFilter.Fatal]
    };

    public static readonly ProgramSettings Release = new()
    {
        WindowSize = (800, 600),
        Title = "[ RELEASE ] OpenGL Engine GameWindow",
        WindowState = WindowState.Maximized,
        WindowBorder = WindowBorder.Resizable,
        LogFilter = [Helpers.LogFilter.Nothing]
    };
}
