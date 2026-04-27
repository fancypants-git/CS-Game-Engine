namespace Windowing;

public static class Program
{
    public static void Main(string[] args)
    {
        _ = new Camera(WindowManager.WindowIdentifier.Window0);
        _ = new Camera(WindowManager.WindowIdentifier.Window1);
        _ = new Camera(WindowManager.WindowIdentifier.Window4);

        Application.Run();
    }
}