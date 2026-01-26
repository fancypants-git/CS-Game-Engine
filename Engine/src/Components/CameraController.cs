using Engine.Attributes;
using Engine.Helpers;
using Engine.Maths;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Engine.Components;

/// <summary>
/// Enables the transformation of mouse movement to the rotation of an object
/// </summary>
[ComponentMeta("CameraController")]
[DisallowMultiple]
public class CameraController : Component
{
    /// <summary>
    /// The sensitivity of the controller (is modified by <see cref="sensitivityConstant"/> to be in a more normal range)
    /// </summary>
    public float Sensitivity;

    /// <summary>
    /// Whether to invert the mouse movement along the Y axis
    /// </summary>
    public bool InvertY;

    /// <summary>
    /// Is used to modify the mouse input into a normal range of movement
    /// </summary>
    private const float sensitivityConstant = 0.01f;

    /// <summary>
    /// The current pitch of the controller
    /// </summary>
    private float _pitch = 0f;

    /// <summary>
    /// The current yaw of the controller
    /// </summary>
    private float _yaw = 0f;

    /// <summary>
    /// Creates a new camera controller
    /// </summary>
    /// <param name="entity">The parent entity</param>
    /// <param name="sensitivity">The sensitivity of this controller</param>
    /// <param name="invertY">Whether to invert the mouse movement along the Y axis</param>
    public CameraController(Entity entity, float sensitivity, bool invertY) : base(entity)
    {
        Sensitivity = sensitivity;
        InvertY = invertY;
    }

    /// <inheritdoc/>
    public override void Update()
    {
        _yaw += -Input.MouseDelta.X * sensitivityConstant * Sensitivity;
        _pitch += (InvertY ? -Input.MouseDelta.Y : -Input.MouseDelta.Y)
            * sensitivityConstant * Sensitivity;
        
        _pitch = Math.Clamp(_pitch, -89, 89);

        Transform.Rotation = Quaternion.FromEulerAngles(0f, _yaw * MathHelper.DegToRad, 0f)
            * Quaternion.FromEulerAngles(_pitch * MathHelper.DegToRad, 0f, 0f);
    }   
}
