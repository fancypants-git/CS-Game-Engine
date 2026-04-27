using OpenTK.Windowing.Desktop;
using Engine.Debugging;

namespace Engine;

/// <summary>
/// Manages and controls windows
/// </summary>
public class WindowManager : IDisposable
{
    public enum WindowIdentifier
    {
        Window0,
        Window1,
        Window2,
        Window3,
        Window4,
        Main = Window0,
    }

    /// <summary>
    /// Stores all active WindowIdentifiers paired with their window instances
    /// </summary>
    private Dictionary<WindowIdentifier, NativeWindow> _activeWindows = new();

    private List<WindowIdentifier> _windowDrawRequests = new();

    private HashSet<WindowIdentifier> _closedWindows = new(); // all windows that were closed this frame

    private bool _isDisposed = false;

    /// <summary>
    /// Whether this WindowManager has been disposed
    /// </summary>
    public bool IsDisposed
    {
        get => _isDisposed;
    }

    /// <summary>
    /// All WindowIdentifiers that were closed this frame
    /// </summary>
    public HashSet<WindowIdentifier> ClosedWindows
    {
        get => _closedWindows;
    }

    /// <summary>
    /// Checks if a WindowIdentifier is active
    /// </summary>
    /// <param name="identifier">The WindowIdentifier that has to be validated</param>
    /// <returns>Whether the identifier is active</returns>
    public bool IsActiveWindow(WindowIdentifier identifier)
    {
        return _activeWindows.ContainsKey(identifier);
    }

    /// <summary>
    /// Checks if a WindowIdentifier is active and exists (thus meaning it is valid)
    /// </summary>
    /// <param name="identifier">The WindowIdentifier that has to be validated</param>
    /// <returns>Whether the identifier is valid</returns>
    public bool IsValidTarget(WindowIdentifier identifier)
    {
        NativeWindow? window = GetWindow(identifier);
        return window != null && window.Exists;
    }

    /// <summary>
    /// Gets the NativeWindow paired with the given WindowIdentifier if it is active
    /// </summary>
    /// <param name="identifier">The requested window's identifier</param>
    /// <returns>The NativeWindow paired with the identifier, null if not active</returns>
    public NativeWindow? GetWindow(WindowIdentifier identifier)
    {
        _activeWindows.TryGetValue(identifier, out NativeWindow? window);
        return window;
    }

    /// <summary>
    /// Gets the NativeWindow paired with the Window0 identifier
    /// </summary>
    /// <returns>The main window</returns>
    public NativeWindow? GetMainWindow()
    {
        return GetWindow(WindowIdentifier.Main);
    }

    /// <summary>
    /// Creates a new NativeWindow if the given WindowIdentifier is not active yet
    /// </summary>
    /// <param name="identifier">The identifier to activate</param>
    /// <param name="settings">The settings for the new NativeWindow</param>
    public void CreateWindow(WindowIdentifier identifier, NativeWindowSettings settings)
    {
        if (_isDisposed) return;

        if (IsActiveWindow(identifier))
        {
            Debug.LogWarn($"Window with identifier {identifier} is already active.");
            return;
        }
        
        settings.Title = identifier.ToString();
        NativeWindow window = new(settings);

        if (identifier == WindowIdentifier.Main)
        {
            window.Closing += (CancelEventArgs) => Application.RequestShutdown();
        }
        else
        {
            window.Closing += (CancelEventArgs) => RegisterWindowClosed(identifier);
        }

        _activeWindows.Add(identifier, window);
    }

    /// <summary>
    /// Registers a draw request for a window if it has not already been registered yet
    /// </summary>
    /// <param name="identifier">The WindowIdentifier to register a request for</param>
    /// <returns>True if the draw request is permitted to continue, false if it should be aborted</returns>
    public bool RegisterWindowDrawRequest(WindowIdentifier identifier)
    {
        if (_windowDrawRequests.Contains(identifier) || _isDisposed)
            return false; // signal that the draw request should be canceled because it has already been registered (or this window manager has been disposed)

        _windowDrawRequests.Add(identifier);
        return true;
    }

    /// <summary>
    /// Registers a window to be closed
    /// </summary>
    /// <param name="identifier">The WindowIdentifier that is closed</param>
    public void RegisterWindowClosed(WindowIdentifier identifier)
    {
        _closedWindows.Add(identifier);
    }

    /// <summary>
    /// Updates all active windows and requests a new event frame
    /// </summary>
    public void UpdateAllWindows()
    {
        if (_isDisposed) return;

        foreach ((_, NativeWindow window) in _activeWindows)
        {
            window.NewInputFrame();
        }

        NativeWindow.ProcessWindowEvents(false);
    }

    /// <summary>
    /// Displays the newest rendered frame on every window
    /// </summary>
    public void RenderAllWindows()
    {
        if (_isDisposed) return;

        foreach ((WindowIdentifier identifier, NativeWindow window) in _activeWindows)
        {
            if (_windowDrawRequests.Contains(identifier))
                window.Context.SwapBuffers();
            else
            {
                window.Close();
                window.Dispose();
                _activeWindows.Remove(identifier);
            }
        }

        _windowDrawRequests.Clear();
        _closedWindows.Clear();
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            foreach ((_, NativeWindow window) in _activeWindows)
            {
                window.Close();
                window.Dispose();
            }

            _activeWindows.Clear();
        }

        _isDisposed = true;
    }

    ~WindowManager()
    {
        if (_isDisposed) return;

        Debug.LogWarn($"No Dispose() called on {typeof(WindowManager)}");
        Dispose(false);
    }

    // TODO: add methods to manipulate the active NativeWindows based on WindowIdentifier
}