using System.ComponentModel;
using System.Diagnostics;
using Engine.Attributes;
using Engine.Helpers;
using Engine.Scene;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Debug = Engine.Helpers.Debug;

namespace Engine;

public class Window : GameWindow
{
    public Window(ProgramSettings settings)
        : base(GameWindowSettings.Default, new NativeWindowSettings {
            API = ContextAPI.OpenGL,
            Profile = ContextProfile.Core,
            Flags = settings.Flags,
            ClientSize = settings.WindowSize,
            Vsync = settings.VSync,
            Title = settings.Title,
            StartFocused = true,
            WindowState = settings.WindowState,
            WindowBorder = settings.WindowBorder,
        })
    {
        Settings = settings;
    }
    
    protected ProgramSettings Settings { get; }
    private double _fixedUpdateWaitingTime = 0;
    

    protected override sealed void OnLoad()
    {
        base.OnLoad();
        
        Debug.LogPrefixed(LogType.Launch, "Initializing OpenGL Context");
        
        GL.ClearColor(0,0,0, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        Input.Initialize(KeyboardState, MouseState);
        
        Debug.LogPrefixed(LogType.Launch, "Finished Initializing OpenGL Context");

        try
        {
            Start();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in Load() |", ex.Message, '\n', ex);
        }
    }


    protected override sealed void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        Time.DeltaTimeDouble = args.Time * Time.TimeScale;

        _fixedUpdateWaitingTime += Time.DeltaTimeDouble;

        while (_fixedUpdateWaitingTime > Settings.FixedUpdateDelta)
        {
            try
            {
                Time.FixedDeltaTimeDouble = _fixedUpdateWaitingTime;
                _fixedUpdateWaitingTime -= Settings.FixedUpdateDelta;
                
                SceneManager.FixedUpdateScene();
                FixedUpdate();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error Occured in FixedUpdate() |", ex.Message, '\n', ex);
            }
        }

        try
        {
            SceneManager.UpdateScene();
            Update();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in Update() |", ex.Message, '\n', ex);
        }
    }


    protected override sealed void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        Time.RenderTimeDouble = args.Time * Time.TimeScale;

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        try
        {
            SceneManager.RenderScene();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in RenderFrame() |", ex.Message, '\n', ex);
        }

        SwapBuffers();
    }


    protected override sealed void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        SceneManager.ActiveCamera.SetViewportSize(e.Width, e.Height);
    }


    protected override sealed void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        try
        {
            OnClose();

            Debug.LogInfo("Disposing Objects");

            SceneManager.Dispose();

            Debug.LogInfo("Finished Disposing Objects");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in UnLoad() |", ex.Message, '\n', ex);
        }
        finally
        {
            Debug.LogPrefixed(LogType.Exit, "Exited Program.");
        }
    }


    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
    }
    
    protected virtual void OnClose()
    {
    }
}

[GameEntry]
internal class Program() : Window(ProgramSettings.Default)
{
    protected override void Start()
    {
        SceneManager.InitializeScene(Resources.GetPath("Scenes/Example.scene"));
        CursorState = CursorState.Grabbed;
    }

    protected override void Update()
    {
        if (Input.IsKeyDown(Keys.Escape))
        {
            Debug.LogPrefixed(LogType.Exit, "Exiting Due to Escape press!");
            Close();
        }
    }
}