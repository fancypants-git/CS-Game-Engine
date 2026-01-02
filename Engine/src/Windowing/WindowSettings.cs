using Engine.Maths;
using OpenTK.Windowing.Common;

namespace Engine.Windowing;

// WindowSettings determine the window/openGL related settings for a single Window Instance

public class WindowSettings
{
    public ContextFlags Flags { get; set; } = ContextFlags.Default;
    public VSyncMode VSync { get; set; } = VSyncMode.Off;
    
    public Vector2 WindowSize { get; set; } = new(800, 600);
    public string Title { get; set; } = "OpenGL Engine Window";
    public WindowState WindowState { get; set; } = WindowState.Normal;
    public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;

    public WindowSettings(WindowSettings from)
    {
        Flags = from.Flags;
        VSync = from.VSync;
        WindowSize = from.WindowSize;
        Title = from.Title;
        WindowState = from.WindowState;
        WindowBorder = from.WindowBorder;
    }
    public WindowSettings() {}

    public static readonly WindowSettings Default = new();

    public static readonly WindowSettings Debug = new()
    {
        Flags = ContextFlags.Debug,
        Title = "[ DEBUG ] OpenGL Engine Window",
    };

    public static readonly WindowSettings Release = new()
    {
        Title = "[ RELEASE ] OpenGL Engine Window",
        WindowState = WindowState.Maximized,
    };
}