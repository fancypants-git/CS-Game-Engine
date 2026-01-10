using Engine.Helpers;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using System.ComponentModel;

namespace Engine.Windowing;

public class Window : NativeWindow
{
    public Window() : base(NativeWindowSettings.Default)
    {
        ID = WindowID.New();
        InputHandler = new(KeyboardState, MouseState);
        Initialize();
    }

    public Window(WindowSettings settings) : base(new NativeWindowSettings())
    {
        ID = WindowID.New();
        InputHandler = new(KeyboardState, MouseState);
        Initialize();
    }

    public readonly WindowID ID;

    public RenderContext RenderContext => new RenderContext()
    {
        WindowID = ID,
        Size = Size,
        AspectRatio = (float)Size.X / Size.Y
    };

    public Input InputHandler;

    public virtual void SetAsCurrentContext()
    {
        Application.WindowManager.SetRenderContext(RenderContext);
    }

    public virtual void Initialize()
    {
        Application.WindowManager.SetMainWindowIfNone(ID);

        SetAsCurrentContext();

        GL.ClearColor(0, 0, 0, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }


    public virtual void Update()
    {
        ProcessWindowEvents(false); // set waitForEvents to false because we dont want to wait for events to happen
    }

    public virtual void Display()
    {
        SetAsCurrentContext();

        GL.Viewport(0, 0, Size.X, Size.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Application.Game.Render();

        Context.SwapBuffers();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        Application.WindowManager.MarkForClose(ID);
    }
}