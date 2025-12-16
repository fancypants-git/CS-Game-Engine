using System.ComponentModel;
using Engine.Helpers;
using Engine.Physics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Engine.Windowing ;

public class Game : GameWindow
{
    protected Game(ProgramSettings settings, GameSettings gameSettings)
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
        
        GameSettings = gameSettings;
        PhysicsHandler.Initialize(gameSettings.Gravity);
    }
    
    protected ProgramSettings Settings { get; }
    protected GameSettings GameSettings { get; }
    
    private double _fixedUpdateWaitingTime = 0;
    

    protected override sealed void OnLoad()
    {
        try
        {
            Awake();
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
        
        Debug.LogPrefixed(LogType.Launch, "Initializing OpenGL Context");
        try
        {
            base.OnLoad();
            GL.ClearColor(0,0,0, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            GL.Viewport(0, 0, Winfo.WindowSize.X, Winfo.WindowSize.Y);
            
            Input.Initialize(KeyboardState, MouseState);
        }
        catch(Exception e)
        {
            Debug.LogFatal("Failed to Initialize OpenGL Context! Exiting progam immediately.");
            Debug.LogFatal(e);
            Close();
            return;
        }
        
        Debug.LogPrefixed(LogType.Launch, "Succesfully Finished Initializing OpenGL Context");
        
        

        try
        {
            Start();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    protected sealed override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        Time.DeltaTimeDouble = args.Time * Time.TimeScale;

        _fixedUpdateWaitingTime += Time.DeltaTimeDouble;

        while (_fixedUpdateWaitingTime > GameSettings.FixedUpdateDelta)
        {
            try
            {
                Time.FixedDeltaTimeDouble = _fixedUpdateWaitingTime;
                _fixedUpdateWaitingTime -= GameSettings.FixedUpdateDelta;
                
                if (GameSettings.PhysicsUpdate == GameSettings.FixedUpdate)
                    PhysicsHandler.World.StepSimulation(Time.FixedDeltaTime);
                SceneManager.FixedUpdateScene();
                FixedUpdate();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        try
        {
            if (GameSettings.PhysicsUpdate == GameSettings.Update)
                PhysicsHandler.World.StepSimulation(Time.DeltaTime);
            SceneManager.UpdateScene();
            Update();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }


    sealed protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        Time.RenderTimeDouble = args.Time * Time.TimeScale;

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        try
        {
            SceneManager.RenderScene();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        SwapBuffers();
    }


    protected sealed override void OnFramebufferResize(FramebufferResizeEventArgs args)
    {
        base.OnFramebufferResize(args);

        GL.Viewport(0, 0, args.Width, args.Height);
        Winfo.WindowSize = (args.Width, args.Height);
        
        try
        {
            SceneManager.ActiveCamera.SetViewportSize(args.Width, args.Height);
        }
        catch(NullReferenceException)
        {
            Debug.LogWarn("No main camera set for currently active scene!");
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }


    protected sealed override void OnClosing(CancelEventArgs args)
    {
        try
        {
            OnClose();

            Debug.LogInfo("Disposing Objects");

            SceneManager.Dispose();

            Debug.LogInfo("Finished Disposing Objects");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            base.OnClosing(args);
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
