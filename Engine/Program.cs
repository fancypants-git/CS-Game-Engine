using Engine;
using Engine.Windowing;
using Engine.Attributes;
using Engine.Helpers;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Engine.Debugging;
using System.Drawing;

[GameEntry]
internal class Program() : Game(new ProgramSettings(ProgramSettings.Debug)
{
    // WindowState = WindowState.Maximized
    FixedUpdateDelta = 0.01
})
{
    protected override void Awake()
    {
    }
    
    protected override void Start()
    {
        try
        {
            SceneManager.InitializeScene(Resources.GetPath("Scenes/Example.scene"));
            CursorState = CursorState.Grabbed;
        }
        finally
        {
            // Debug.LogPrefixed(LogType.Debug, "Closing Window");
            // Close();
        }
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
