using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine;

public class Window : GameWindow
{
    public Window(int width, int height, string title)
        : base(GameWindowSettings.Default, new NativeWindowSettings {
            API = ContextAPI.OpenGL,
            Profile = ContextProfile.Core,
            Flags = ContextFlags.Debug,
            ClientSize = (width, height),
            Vsync = VSyncMode.Off,
            Title = title,
            StartFocused = false,
            WindowState = WindowState.Normal,
            WindowBorder = WindowBorder.Resizable,
        })
    { }


    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0,0,0, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        Input.Initialize(KeyboardState, MouseState);

        CursorState = CursorState.Grabbed;

        try
        {
            var scene = Resources.GetScene("Scenes/PhysicsTest.scene");
            SceneManager.ActivateScene(scene, false);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in OnLoad() |", ex.Message, '\n', ex);
        }
        
        SceneManager.ActiveCamera?.SetViewportSize(ClientSize.X, ClientSize.Y);
    }


    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        Time.DeltaTimeDouble = args.Time * Time.TimeScale;

        try
        {
            SceneManager.ActiveScene?.EarlyUpdate();
            
            SceneManager.ActiveScene?.Update();
            
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Debug.LogPrefixed(LogType.Exit, "Exiting due to Escape Press");
                Close();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in UpdateFrame() |", ex.Message, '\n', ex);
        }
    }


    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        Time.RenderTimeDouble = args.Time * Time.TimeScale;

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        try
        {
            SceneManager.ActiveScene?.Render();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in RenderFrame() |", ex.Message, '\n', ex);
        }

        SwapBuffers();
    }


    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        
        SceneManager.ActiveCamera.SetViewportSize(e.Width, e.Height);
    }


    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        try
        {
            Debug.LogInfo("Disposing Objects");

            SceneManager.ActiveScene.Dispose();

            Debug.LogInfo("Finished Disposing Objects");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in UnLoad() |", ex.Message, '\n', ex);
        }
    }
}

internal static class Launch
{
    private static void Main(string[] args)
    {
        Debug.LogFilter = [LogFilter.Debug, LogFilter.Warning, LogFilter.Error, LogFilter.Fatal];
        
        Debug.LogPrefixed(LogType.Launch, "Launching Program, Creating Window");
        
        using Window window = new(600, 600, "OpenTK Game Engine");
        window.Run();
        
        Debug.LogPrefixed(LogType.Exit, "Closed Window, Exiting Program");
    }
}