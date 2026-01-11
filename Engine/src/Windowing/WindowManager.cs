using Engine.Rendering;

namespace Engine.Windowing;

public class WindowManager
{
    private Dictionary<WindowID, Window> _allWindows = [];
    private HashSet<WindowID> _requestedForClosing = [];

    public int Count => _allWindows.Count;

    public RenderContext? RenderContext { get; private set; }

    public void SetRenderContext(RenderContext context)
    {
        if (!_allWindows.TryGetValue(context.WindowID, out Window? window))
            return;

        RenderContext = context;
        window.MakeCurrent();
    }


    public Window? GetWindow(WindowID id)
    {
        if (_allWindows.TryGetValue(id, out Window? window))
            return window;

        return null;
    }

    public void CreateWindow(WindowSettings settings)
    {
        Window newWindow = new(settings);
        _allWindows.Add(newWindow.ID, newWindow);
    }

    public void MarkForClose(WindowID id)
    {
        _requestedForClosing.Add(id);
    }

    public void CloseWindow(WindowID id)
    {
        GetWindow(id)?.Close();
    }

    public void CloseAllWindows()
    {
        foreach (Window window in _allWindows.Values)
        {
            window.Close();
        }

        Application.RequestShutdown();
    }

    public void UpdateAllWindows()
    {
        foreach (Window window in _allWindows.Values)
        {
            window.Update();
        }

        foreach (WindowID id in _requestedForClosing)
        {
            GetWindow(id)?.Dispose();
            _allWindows.Remove(id);
            if (RenderContext.HasValue && RenderContext.Value.WindowID.Equals(id))
                RenderContext = null;
        }

        _requestedForClosing.Clear();
    }

    public void DisplayAllWindows()
    {
        foreach (Window window in _allWindows.Values)
        {
            // if the window is hidden, there is no reason to render it
            if (window.WindowState != OpenTK.Windowing.Common.WindowState.Minimized)
                window.Display();
        }
    }
}