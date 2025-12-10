using Engine;
using Engine.Windowing;
using Engine.Attributes;
using Engine.Helpers;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

[GameEntry]
internal class Program() : Game(new ProgramSettings(ProgramSettings.Debug)
{
    // WindowState = WindowState.Maximized
})
{
    protected override void Awake()
    {
    }
    
    protected override void Start()
    {
        SceneManager.InitializeScene(Resources.GetPath("Scenes/Example.scene"));
        CursorState = CursorState.Grabbed;
    }

    protected override void Update()
    {
        if (Input.IsKeyPressed(Keys.Escape))
        {
            if (CursorState == CursorState.Grabbed)
            {
                CursorState = CursorState.Normal;
                Time.TimeScale = 0;
            }
            else
            {
                CursorState = CursorState.Grabbed;
                Time.TimeScale = 1;
            }
        }
    }
}
