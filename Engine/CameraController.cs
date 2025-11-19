using OpenTK.Mathematics;

namespace Engine;

[ComponentMeta("CameraController")]
[DisallowMultiple(true)]
public class CameraController : Component
{
    public float Sensitivity;
    public bool InvertY;

    public CameraController(Entity parent, float sensitivity, bool invertY) : base(parent)
    {
        Sensitivity = sensitivity;
        InvertY = invertY;
    }

    public override void Update()
    {
        base.Update();
        
        Transform.Rotate(-Input.MouseDelta.X / 100f * Sensitivity, Vector3.UnitY);
        if (InvertY)
            Transform.RotateXClamped(Input.MouseDelta.Y / 100f * Sensitivity, -89, 89);
        else
            Transform.RotateXClamped(-Input.MouseDelta.Y / 100f * Sensitivity, -89, 89);
    }
}