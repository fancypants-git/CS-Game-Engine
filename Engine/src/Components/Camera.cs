using Engine.Attributes;
using Engine.Debugging;
using Engine.Interfaces;
using Engine.Maths;
using Matrix4 = OpenTK.Mathematics.Matrix4;
using MathHelper = OpenTK.Mathematics.MathHelper;
using Engine.Helpers;
using Engine.Rendering;

namespace Engine.Components;

[ComponentMeta("Camera")]
public class Camera : Component
{
    public enum CameraType
    {
        Perspective,
        Orthographic
    }

    public CameraType Type;

    // Perspective Camera Variables
    private float _fovy;
    public float Fovy
    {
        get
        {
            return _fovy;
        }
        set
        {
            _fovy = Math.Clamp(value, 1f, 179f);
        }
    }

    // Orthographic Camera Variables
    public Vector2 Size;

    // Global Camera Variables
    public float MaxDepth;
    public float MinDepth;

    public Matrix4 Projection { get; private set; }
    public Matrix4 View { get; private set; }

    public Camera(Entity entity) : base(entity)
    {
        Type = CameraType.Perspective;
        Fovy = 90.0f;
        MaxDepth = 1000f;
        MinDepth = 0.1f;
    }

    public Camera(Entity entity, CameraType cameraType, float minDepth, float maxDepth, float fovy = 90.0f, Vector2? size = null)
        : base(entity)
    {
        Type = cameraType;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
        Fovy = fovy;
        Size = size ?? Vector2.One;
    }
    public Camera(Entity entity, int cameraType, float minDepth, float maxDepth, float fovy, Vector2 size)
        : base(entity)
    {
        Type = (CameraType)cameraType;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
        Fovy = fovy;
        Size = size;
    }

    public void Render(IDrawable[] drawables)
    {
        if (!Application.WindowManager.RenderContext.HasValue)
            return;

        View = Matrix4.LookAt(Transform.Position, Transform.Position + Transform.Forwards, Vector3.UnitY);
        Projection = Type switch
        {
            CameraType.Perspective => Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegToRad * Fovy, Application.WindowManager.RenderContext.Value.AspectRatio, MinDepth, MaxDepth),
            CameraType.Orthographic => Matrix4.CreateOrthographic(Size.X, Size.Y, MinDepth, MaxDepth),
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var drawable in drawables)
        {
            drawable.Draw(this);
        }
    }
}
