using Engine.Attributes;
using Engine.Helpers;
using Engine.Maths;

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
        
        Transform.Rotate(Vector3.UnitY, -Input.MouseDelta.X / 100f * Sensitivity);
        if (InvertY)
            Transform.Rotate(Vector3.UnitX, Input.MouseDelta.Y / 100f * Sensitivity);
        else
            Transform.Rotate(Vector3.UnitX, -Input.MouseDelta.Y / 100f * Sensitivity);
    }
}
