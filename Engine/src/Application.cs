using Stopwatch = System.Diagnostics.Stopwatch;
using Engine.Helpers;
using Engine.Debugging;
using Engine.Windowing;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine;

public static class Application
{
    public static bool IsRunning { get; private set; } = false;

    public static Game Game= new();
    public static WindowManager WindowManager = new();
    public static ApplicationSettings Settings;

    private static Stopwatch _stopwatch = new();

    /// <summary>
    /// Calls the application to start running
    /// </summary>
    public static void Run()
    {
        IsRunning = true;

        Game.Load();
        MainLoop();
    }

    /// <summary>
    /// Requests the application to stop running
    /// </summary>
    public static void RequestShutdown()
    {
        IsRunning = false;
    }

    private static void Shutdown()
    {
        if (IsRunning)
        {
            Debug.LogErr("Application Shutdown called while IsRunning was still true! Cancelling shutdown.");
            Debug.LogInfo("To Request an Application Shutdown please call RequestShutdown()");
            return;
        }

        _stopwatch.Stop();
        WindowManager.CloseAllWindows();
        Game.Close();
    }

    /// <summary>
    /// The Main Loop
    /// </summary>
    private static void MainLoop()
    {
        _stopwatch.Start();
        long frameStart = 0L, fixedFrameStart = 0L;

        while (IsRunning)
        {
            Time.DeltaTimeMilliseconds = (int)((_stopwatch.ElapsedMilliseconds - frameStart) * Time.TimeScale);
            frameStart = _stopwatch.ElapsedMilliseconds;

            Game.SceneManager.LoadRequestedScene();

            // fixed update
            long elapsedMillisecondsSinceFixedUpdate = _stopwatch.ElapsedMilliseconds - fixedFrameStart;
            while (elapsedMillisecondsSinceFixedUpdate >= Game.Settings.FixedUpdateInterval)
            {
                // calculate new fixed update times
                Time.FixedDeltaTimeMilliseconds = (int)((_stopwatch.ElapsedMilliseconds - fixedFrameStart) * Time.TimeScale);
                fixedFrameStart = _stopwatch.ElapsedMilliseconds;
                elapsedMillisecondsSinceFixedUpdate -= Game.Settings.FixedUpdateInterval;

                Game.FixedUpdate();
            }

            WindowManager.UpdateAllWindows();
            Game.Update();

            WindowManager.DisplayAllWindows();

            if (WindowManager.Count == 0 && !)
                RequestShutdown();
        }

        Shutdown();
    }
}