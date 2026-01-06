namespace Engine.Windowing;

public class WindowManager
{
    private Dictionary<WindowID, Window> _allWindows = [];

    public WindowID? MainWindowID { get; private set; }
    public Window? GetMainWindow()
    {
        if (MainWindowID == null) return null;

        return GetWindow(MainWindowID);
    }

    public void SetMainWindow(WindowID id)
    {
        MainWindowID = id;
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
    }

    public void DisplayAllWindows()
    {
        foreach (Window window in _allWindows.Values)
        {
            window.Display();
        }
    }
}