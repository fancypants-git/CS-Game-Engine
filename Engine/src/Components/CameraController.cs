using Engine.Attributes;
using Engine.Debugging;
using Engine.Helpers;
using Engine.Maths;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Engine.Components;

[ComponentMeta("CameraController")]
[DisallowMultiple]
public class CameraController : Component
{
    public float Sensitivity;
    public bool InvertY;
    private const float sensitivityConstant = 0.01f;

    private float _pitch = 0f;
    private float _yaw = 0f;

    public CameraController(Entity entity, float sensitivity, bool invertY) : base(entity)
    {
        Sensitivity = sensitivity;
        InvertY = invertY;
    }

    public override void Update()
    {
        Input? input = Application.WindowManager.GetMainWindow()?.InputHandler;

        if (input == null)
            return;

        _yaw += -input.MouseDelta.X * sensitivityConstant * Sensitivity;
        _pitch += (InvertY ? -input.MouseDelta.Y : input.MouseDelta.Y)
            * sensitivityConstant * Sensitivity;
        
        _pitch = Math.Clamp(_pitch, -89, 89);

        Transform.Rotation = Quaternion.FromEulerAngles(0f, _yaw * MathHelper.DegToRad, 0f)
            * Quaternion.FromEulerAngles(_pitch * MathHelper.DegToRad, 0f, 0f);
    }
}
