using Engine.Helpers;
using OpenTK.Graphics.OpenGL;
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
    }

    public void Update()
    {
        ProcessWindowEvents(false);
    }

    public void Display()
    {
        // no use of displaying if the window is not visible, just wastes precious resources
        if (WindowState == OpenTK.Windowing.Common.WindowState.Minimized) return;

        MakeCurrent();

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Application.GetGameInstance().Render();

        Context.SwapBuffers();
    }


}