using Engine.Helpers;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Windowing.Desktop;

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

    public Input InputHandler;



    public virtual void Initialize()
    {
        MakeCurrent();

        GL.ClearColor(0, 0, 0, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }


    public virtual void Update()
    {
        MakeCurrent();
        ProcessWindowEvents(false); // set waitForEvents to false because we dont want to wait for events to happen
    }

    public virtual void Display()
    {
        MakeCurrent();

        GL.Viewport(Location.X, Location.Y, Size.X, Size.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Application.Game.Render(new RenderContext()
        {
            Size = Size,
            AspectRatio = Size.X / Size.Y
        });

        Context.SwapBuffers();
    }
}