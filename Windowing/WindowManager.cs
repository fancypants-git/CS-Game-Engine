using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Windowing;

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
        Window4
    }

    /// <summary>
    /// Stores all active WindowIdentifiers paired with their window instances
    /// </summary>
    private Dictionary<WindowIdentifier, NativeWindow> _activeWindows = new();

    private bool _isDisposed = false;

    /// <summary>
    /// Whether this WindowManager has been disposed
    /// </summary>
    public bool IsDisposed
    {
        get => _isDisposed;
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

    public bool IsValidTarget(WindowIdentifier identifier)
    {
        NativeWindow? window = GetWindow(identifier);
        if (window == null) return false;
        return window.Exists;
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
        return GetWindow(WindowIdentifier.Window0);
    }

    /// <summary>
    /// Creates a new NativeWindow if the given WindowIdentifier is not active yet
    /// </summary>
    /// <param name="identifier">The identifier to activate</param>
    /// <param name="settings">The settings for the new NativeWindow</param>
    public void CreateWindow(WindowIdentifier identifier, NativeWindowSettings settings)
    {
        if (IsActiveWindow(identifier))
        {
            Debug.LogWarn($"Window with identifier {identifier} is already active.");
            return;
        }

        NativeWindow window = new(settings);

        _activeWindows.Add(identifier, window);
    }

    /// <summary>
    /// Updates all active windows and requests a new event frame
    /// </summary>
    public void UpdateAllWindows()
    {
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
        foreach ((_, NativeWindow window) in _activeWindows)
        {
            window.Context.SwapBuffers();
        }
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