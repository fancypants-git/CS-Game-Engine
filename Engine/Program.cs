using Engine;
using Engine.Windowing;
using Engine.Attributes;
using Engine.Helpers;
using Engine.Debugging;
using Engine.Maths;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

[GameEntry]
internal class Program() : Game(ProgramSettings.Debug, new(GameSettings.Default)
{
    FixedUpdateDelta = 0.01f,
})
{
    protected override void Awake()
    {
    }
    
    protected override void Start()
    {
        SceneManager.InitializeScene(Resources.GetPath("Scenes/BodyTest.scene"));
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

        const float length = 100f;
        Debug.DrawLine(Vector3.Zero, Vector3.UnitX * length, Color.Red);
        Debug.DrawLine(Vector3.Zero, Vector3.UnitY * length, Color.Blue);
        Debug.DrawLine(Vector3.Zero, Vector3.UnitZ * length, Color.Green);
    }
}
