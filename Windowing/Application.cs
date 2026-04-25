using System.Diagnostics;

namespace Windowing;

/// <summary>
/// Manages the main loop and lifetime of the program
/// </summary>
public static class Application
{
    private static bool _isRunning = false;
    private static Stopwatch _timer = new();
    private static WindowManager _windowManager = new();

    /// <summary>
    /// Whether the Application is running
    /// </summary>
    public static bool IsRunning
    {
        get => _isRunning;
    }

    /// <summary>
    /// The timer used for handling delta time and frame timing
    /// </summary>
    public static Stopwatch ApplicationTimer
    {
        get => _timer;
    }

    /// <summary>
    /// The WindowManager of this application
    /// </summary>
    public static WindowManager WindowManager
    {
        get => _windowManager;
    }

    /// <summary>
    /// Requests the Application to be shut down as soon as possible
    /// </summary>
    public static void RequestShutdown()
    {
        _isRunning = false;
    }

    /// <summary>
    /// Initializes the Application and calls Mainloop
    /// </summary>
    public static void Run()
    {
        _isRunning = true;
        _timer.Start();
        Mainloop();
    }

    /// <summary>
    /// The main Application loop, this loop handels everything that happens inside of a frame
    /// </summary>
    private static void Mainloop()
    {
        long lastFrameStartTime = _timer.ElapsedMilliseconds;
        while (_isRunning)
        {
            _windowManager.UpdateAllWindows();
            // fixed game update
            // game update
            // game render
            _windowManager.RenderAllWindows();
        }

        Dispose();
    }

    /// <summary>
    /// Disposes and finalizes the Application
    /// </summary>
    private static void Dispose()
    {
        if (_isRunning)
        {
            Debug.LogWarn("Canceling Dispose request since Application is still running.");
            return;
        }

        _timer.Stop();
        _windowManager.Dispose();
    }
}