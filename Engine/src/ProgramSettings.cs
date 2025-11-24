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
    public WindowState WindowState { get; set; } = WindowState.Maximized;
    public WindowBorder WindowBorder { get; set; } = WindowBorder.Resizable;


    public readonly static ProgramSettings Default = new();
    
    public readonly static ProgramSettings Debug = new() {
        Flags = ContextFlags.Debug,
        WindowSize = (800, 600),
        Title = "[ DEBUG ] OpenGL Engine GameWindow",
        WindowState = WindowState.Normal,
        WindowBorder = WindowBorder.Resizable
    };
}