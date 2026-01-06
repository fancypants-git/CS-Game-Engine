using Engine.Helpers;
using Engine.Windowing;

namespace Engine;

public class Program
{
    public static void Main(string[] args)
    {
        Application.WindowManager.CreateWindow(WindowSettings.Default);
        Application.Game.Settings.InitialScene = Resources.GetPath("Scenes/PhysicsTest.scene");
        Application.Run();
    }
}