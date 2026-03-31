using System.ComponentModel;
using Engine.Debugging;
using Engine.Helpers;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Engine.Windowing;

public class Window : NativeWindow
{
    public Window(WindowSettings settings)
        : base(new NativeWindowSettings()
        {
            Flags = settings.Flags,
            Vsync = settings.VSync,
            ClientSize = (OpenTK.Mathematics.Vector2i)settings.WindowSize,
            WindowState = settings.WindowState,
            WindowBorder = settings.WindowBorder,
            Title = settings.Title
        })
    {
        MakeCurrent();
        Input.Initialize(KeyboardState, MouseState);

        GL.ClearColor(0, 0, 0, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        RenderContext = new();
        GpuResourceManager.InitializeResources(RenderContext);
    }

    public readonly RenderContext RenderContext;

    public virtual void Update()
    {
        NewInputFrame();
        ProcessWindowEvents(false);
    }

    public virtual void Display()
    {
        // no use of displaying if the window is not visible, just wastes precious resources
        if (WindowState == WindowState.Minimized) return;

        MakeCurrent();

        GpuResourceManager.InitializeResources(RenderContext);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        Application.Game.Render();
        Context.SwapBuffers();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        Application.RequestCloseWindow();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    public override void Dispose()
    {
        RenderContext.Dispose();
        base.Dispose();
        RenderContext.IsAlive = false;
    }
}