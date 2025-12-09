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

public class Game : GameWindow
{
    protected Game(ProgramSettings settings)
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
        Winfo.WindowSize = ClientSize;
        Debug.LogFilter = settings.LogFilter;
    }
    
    protected ProgramSettings Settings { get; }
    private double _fixedUpdateWaitingTime = 0;
    

    protected override sealed void OnLoad()
    {
        Awake();
        
        base.OnLoad();
        
        Debug.LogPrefixed(LogType.Launch, "Initializing OpenGL Context");
        
        GL.ClearColor(0,0,0, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);
        
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        GL.Viewport(0, 0, Winfo.WindowSize.X, Winfo.WindowSize.Y);
        
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


    protected sealed override void OnUpdateFrame(FrameEventArgs args)
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


    protected sealed override void OnRenderFrame(FrameEventArgs args)
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


    protected sealed override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);

        GL.Viewport(0, 0, e.Width, e.Height);
        SceneManager.ActiveCamera.SetViewportSize(e.Width, e.Height);
        Winfo.WindowSize = (e.Width, e.Height);
    }


    protected sealed override void OnClosing(CancelEventArgs e)
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


    protected virtual void Awake()
    {
        
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
internal class Program() : Game(new ProgramSettings(ProgramSettings.Debug)
{
    // WindowState = WindowState.Maximized
})
{
    protected override void Awake()
    {
    }
    
    protected override void Start()
    {
        // try
        // {
            SceneManager.InitializeScene(Resources.GetPath("Scenes/Example.scene"));
            CursorState = CursorState.Grabbed;
        // }
        // finally
        // {
        //     Close();
        // }
    }

    protected override void Update()
    {
        if (Input.IsKeyPressed(Keys.Escape))
        {
            if (CursorState == CursorState.Grabbed)
            {
                CursorState = CursorState.Normal;
                Time.TimeScale = 0;
            }
            else
            {
                CursorState = CursorState.Grabbed;
                Time.TimeScale = 1;
            }
        }
    }
}