using System.Drawing;
using Engine.Attributes;
using Engine.Debugging;
using Engine.Helpers;
using Engine.Maths;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Components;

[ComponentMeta("RotationTest")]
[DisallowMultiple]
public class RotationTest : Component
{
    public RotationTest(Entity e) : base(e)
    {
        
    }

    private const float rotationSpeedDeg = 90f;
    private readonly Vector3 rotationAxis = new Vector3(1, 1, 0);
    private const float debugLineLength = 5f;
    private bool ShouldRotate = true;

    public override void Update()
    {
        // if (Input.IsKeyPressed(Keys.Backslash))
        //     ShouldRotate = !ShouldRotate;

        if (ShouldRotate)
            Transform.Rotate(rotationAxis, rotationSpeedDeg * MathHelper.DegToRad * Time.DeltaTime, Space.World);

        Debug.DrawLine(Transform.Position, Transform.Position + (Transform.Forwards * debugLineLength), Color.Cyan);
        Debug.DrawLine(Transform.Position, Transform.Position + (Transform.Right * debugLineLength), Color.Magenta);
        Debug.DrawLine(Transform.Position, Transform.Position + (Transform.Up * debugLineLength), Color.Yellow);
        Debug.DrawLine(Transform.Position, Transform.Position + (rotationAxis * debugLineLength), Color.White);
    }
}