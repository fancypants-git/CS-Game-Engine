using Engine.Attributes;
using Engine.Helpers;
using OpenTK.Mathematics;

namespace Engine.Components;

[ComponentMeta("CameraController")]
[DisallowMultiple]
public class CameraController : Component
{
    public float Sensitivity;
    public bool InvertY;

    public CameraController(Entity entity, float sensitivity, bool invertY) : base(entity)
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