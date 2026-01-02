namespace Engine.Windowing;

public class WindowManager
{
    private Dictionary<WindowID, Window> _windowReferences;

    public Window Main { get; set; }

    public Window? LookupWithID(WindowID windowID)
    {
        if (_windowReferences.TryGetValue(windowID, out Window? window))
            return window;
        
        return null;
    }

    public void AddWindow(Window window)
    {
        _windowReferences.Add(window.ID, window);
    }

    public void CreateWindow()
    {
        throw new NotImplementedException();
    }


    // TODO implement window delegation
    public void UpdateAllWindows()
    {
        foreach (var window in _windowReferences.Values)
        {
            window.Update();
        }
    }

    public void FixedUpdateAllWindows()
    {
        foreach (var window in _windowReferences.Values)
        {
            window.FixedUpdate();
        }
    }

    public void RenderAllWindows()
    {
        foreach (var window in _windowReferences.Values)
        {
            
        }
    }
}