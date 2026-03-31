using Engine.Attributes;
using Engine.Debugging;
using Engine.Rendering;
using Engine.Maths;
using Matrix4 = OpenTK.Mathematics.Matrix4;
using MathHelper = OpenTK.Mathematics.MathHelper;

namespace Engine.Components;

[ComponentMeta("Camera")]
public class Camera : Component
{
    public static Camera Main => Application.Game.ActiveCamera;

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

    public Matrix4 StashedView { get; private set; }
    public Matrix4 StashedProjection { get; private set; }

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

    public virtual Matrix4 GetView()
    {
        Matrix4 view = Matrix4.LookAt(Transform.Position, Transform.Position - Transform.Forwards, Vector3.UnitY);
        return view;
    }
    public virtual Matrix4 GetProjection()
    {
        float aspect = (float)Application.Window!.ClientSize.X / Application.Window!.ClientSize.Y;

        if (aspect <= float.Epsilon)
        {
            Debug.LogErr($"Aspect Ratio can not be 0 or less, something very weird mustve happened to achieve this result...\nAnyway, canceling rendering camera of {Entity.Name}");
            return Matrix4.MultiplicativeIdentity;
        }

        return Type switch
        {
            CameraType.Perspective => Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegToRad * Fovy, aspect, MinDepth, MaxDepth),
            CameraType.Orthographic => Matrix4.CreateOrthographic(Size.X, Size.Y, MinDepth, MaxDepth),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void Render(params IDrawable[] drawables)
    {
        StashedView = GetView();
        StashedProjection = GetProjection();

        foreach (var drawable in drawables)
        {
            drawable.Draw(this);
        }
    }
}
