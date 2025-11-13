using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine;

public class Window : GameWindow
{
    private Scene scene1;
    private Scene scene2;
    private Entity cube;
    
    public Window(int width, int height, string title)
        : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            ClientSize = (width, height),
            Title = title
        })
    { }


    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.4f, 0.2f, 0.5f, 1.0f);

        GL.Enable(EnableCap.DepthTest);
        GL.DepthFunc(DepthFunction.Less);

        try
        {
            var shader = Resources.GetShader("default");
            var texture = new Texture(Resources.GetPath("Textures/wall.jpg"), true);

            // Scene 1
            scene1 = new Scene("Scene 1");

            cube = new Entity();
            var r = new Renderer(cube, shader, texture, null);
            cube.AddComponent(r);

            var camera = new Entity();
            var c = new Camera(camera, Camera.CameraType.Perspective, 0.1f, 100f, fovy: 90f);
            camera.AddComponent(c);
            c.SetViewportSize(ClientSize.X, ClientSize.Y);
            
            scene1.AddEntity(cube);
            scene1.AddEntity(camera);
            scene1.AddDrawable(r);
            scene1.ActiveCamera = c;
            scene1.Initialize();
            
            // Scene 2
            scene2 = new Scene("Scene 2");
            
            var cube2 = new Entity();
            cube2.Transform.Translate(0, 0, 5);
            var r2 = new Renderer(cube2, shader, null, Color.DarkGreen);
            cube2.AddComponent(r2);
            
            var camera2 = new Entity();
            var c2 = new Camera(camera2, Camera.CameraType.Perspective, 0.1f, 100, fovy: 30);
            camera2.AddComponent(c2);
            c2.SetViewportSize(ClientSize.X, ClientSize.Y);
            
            scene2.AddEntity(cube2);
            scene2.AddEntity(camera2);
            scene2.AddDrawable(r2);
            scene2.ActiveCamera = c2;
            scene2.Initialize();
            
            SceneManager.ActivateScene(scene1);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in OnLoad() |", ex.Message, '\n', ex);
        }
    }


    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        try
        {
            SceneManager.ActiveScene?.EarlyUpdate();

            // const float speed = 3;
            // if (KeyboardState.IsKeyDown(Keys.W))
            //     camera.Transform.Translate(speed * (float)args.Time, camera.Transform.Forwards);
            // if (KeyboardState.IsKeyDown(Keys.S))
            //     camera.Transform.Translate(speed * (float)args.Time, -camera.Transform.Forwards);
            // if (KeyboardState.IsKeyDown(Keys.D))
            //     camera.Transform.Translate(speed * (float)args.Time, -camera.Transform.Right);
            // if (KeyboardState.IsKeyDown(Keys.A))
            //     camera.Transform.Translate(speed * (float)args.Time, camera.Transform.Right);
            // if (KeyboardState.IsKeyDown(Keys.Space))
            //     camera.Transform.Translate(speed * (float)args.Time, camera.Transform.Up);
            //
            // if (KeyboardState.IsKeyDown(Keys.LeftShift))
            //     camera.Transform.Translate(speed * (float)args.Time, -camera.Transform.Up);
            //
            //
            // var x = (float)(args.Time * 300);
            // var y = (float)(Math.Sqrt(args.Time) * 2f);
            // var z = (float)(Math.Sin(args.Time) * 1);
            // cube.Transform.Rotate(x, y, z);
            
            SceneManager.ActiveScene?.Update();
            
            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Debug.LogPrefixed(Debug.LogType.Exit, "Exiting Due to Escape Press");
                Close();
            }

            if (KeyboardState.IsKeyPressed(Keys.Tab))
            {
                Debug.LogInfo("Switching scene!!");
                switch (SceneManager.ActiveScene?.Name)
                {
                    case "Scene 1":
                        SceneManager.ActivateScene(scene2);
                        break;
                    case "Scene 2":
                        SceneManager.ActivateScene(scene1);
                        break;
                }
                
                SceneManager.ActiveCamera.SetViewportSize(ClientSize.X, ClientSize.Y);
            }
            
            cube.Transform.Translate(0, 0, 2 * (float)args.Time);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error Occured in UpdateFrame() |", ex.Message, '\n', ex);
        }
    }


    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

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
        Debug.LogPrefixed(Debug.LogType.Launch, "Launching Program, Creating Window");
        
        using Window window = new(600, 600, "OpenTK Game Engine");
        window.Run();
        
        Debug.LogPrefixed(Debug.LogType.Exit, "Closed Window, Exiting Program");
    }
}