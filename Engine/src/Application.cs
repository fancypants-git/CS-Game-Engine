using Stopwatch = System.Diagnostics.Stopwatch;
using Engine.Helpers;
using Engine.Debugging;

namespace Engine.Windowing;

// Application: (static)
//  Contains Program loop
//  Contains Game
//  Contains WindowManager
// 
// Game:
//  Contains game update logic
//  Contains SceneManager
//  
// WindowManager:
//  Contains the Active Window
//  Contains Logic to Switch and Create windows
//      - Create / Open Window
//      - Close Window
// 
// Window:
//  Contains Window Update logic
//  Contains Window Display logic
//  Contains OpenGL Context

public static class Application
{
    public static bool IsRunning { get; private set; }

    private static ApplicationSettings Settings;

    private static Game? _requestedGameOverride = null;
    private static Game _game = new();

    private static WindowSettings? _requestedWindowOverride = null;
    private static Window? _window = null;
    public static Window? Window => _window;

    private static readonly Stopwatch _stopwatch = new();

    public static ApplicationSettings GetSettings()
    {
        return Settings;
    }

    public static void SetSettings(ApplicationSettings newSettings)
    {
        if (!IsRunning)
        {
            Settings = newSettings;
        }
    }

    public static Game GetGameInstance()
    {
        return _game;
    }

    public static void RequestOverrideGameInstance(Game newGame)
    {
        if (!IsRunning)
        {
            _game = newGame;
            _requestedGameOverride = null;
        }
        else
            _requestedGameOverride = newGame;
    }

    public static void RequestOverrideWindowInstance(WindowSettings? settings)
    {
        if (!IsRunning && settings != null)
        {
            _window = new(settings);
            _requestedWindowOverride = null;
        }
        else
            _requestedWindowOverride = settings;
    }

    private static void ApplyOverrideRequests()
    {
        if (_requestedGameOverride != null)
        {
            _game.Close();
            _game = _requestedGameOverride;
            _requestedGameOverride = null;

            if (IsRunning)
                _game.Start();

        }

        if (_requestedWindowOverride != null)
        {
            _window?.Close();
            _window?.Dispose();
            _window = new(_requestedWindowOverride);
            _requestedWindowOverride = null;
        }
    }

    public static void RequestShutdown()
    {
        IsRunning = false;
    }

    private static void Shutdown()
    {
        if (IsRunning)
        {
            return;
        }

        _stopwatch.Stop();
        _window?.Close();
        _window?.Dispose();
        _game.Close();
    }

    public static void Run()
    {
        Debug.LogFilter = Settings.LogFilter;
        if (!Settings.RunHeadless)
            RequestOverrideWindowInstance(new(Settings.WindowSettings));
        MainLoop();
    }

    private static void MainLoop()
    {
        IsRunning = true;
        _game.Start();

        long lastFrameStart = 0L;
        long lastFixedFrameStart = 0L;

        _stopwatch.Start();
        while (IsRunning)
        {
            long measuredTime = _stopwatch.ElapsedMilliseconds;
            Time.UpdateDeltaTime(measuredTime - lastFrameStart);
            lastFrameStart = measuredTime;

            // Fixed Update
            // If the total time waited from the last Fixed Update frame is longer than the set time to wait, keep updating it untill it is less
            long waitingTime = _stopwatch.ElapsedMilliseconds - lastFixedFrameStart;
            while (waitingTime >= _game.Settings.FixedUpdateInterval)
            {
                Time.UpdateFixedDeltaTime(waitingTime);
                waitingTime -= _game.Settings.FixedUpdateInterval;

                _game.FixedUpdate();
            }

            _window?.Update();
            _game.Update();

            _window?.Display();
        }

        Shutdown();
    }
}