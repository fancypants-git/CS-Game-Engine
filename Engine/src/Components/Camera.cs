using Engine.Attributes;
using Engine.Debugging;
using Engine.Rendering;
using Engine.Maths;
using Matrix4 = OpenTK.Mathematics.Matrix4;
using MathHelper = OpenTK.Mathematics.MathHelper;

namespace Engine.Components;

/// <summary>
/// Represents a camera and can render a list of IDrawables to the screen.<br/>
/// TODO: rework the camera system to work properly and not be hardcoded to the screen etc.
/// </summary>
[ComponentMeta("Camera")]
public class Camera : Component
{
    /// <summary>
    /// The current main camera in the game
    /// </summary>
    public static Camera Main => Application.Game.ActiveCamera;

    public enum CameraType
    {
        Perspective,
        Orthographic
    }

    /// <summary>
    /// What type of camera this is (see <see cref="CameraType"/>)
    /// </summary>
    public CameraType Type;


    // Perspective Camera Variables

    /// <summary>
    /// The field of view angle (in degrees) of the y axis of the screen
    /// </summary>
    private float _fovy;
    /// <inheritdoc cref="_fovy" />
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

    /// <summary>
    /// The size of the camera clip box's X and Y
    /// </summary>
    public Vector2 Size;


    // Global Camera Variables

    /// <summary>
    /// The furthest the camera can look (max clip distance)
    /// </summary>
    public float MaxDepth;

    /// <summary>
    /// The closest the camera can look (min clip distance). Can not be 0.
    /// </summary>
    public float MinDepth;

    /// <summary>
    /// The latest stashed view matrix
    /// </summary>
    public Matrix4 StashedView { get; private set; }
    /// <summary>
    /// The latest stashed projection matrix
    /// </summary>
    public Matrix4 StashedProjection { get; private set; }

    /// <summary>
    /// Creates a new default perspective camera with FOV 90
    /// </summary>
    /// <param name="entity">The parent entity</param>
    public Camera(Entity entity) : base(entity)
    {
        Type = CameraType.Perspective;
        Fovy = 90.0f;
        MaxDepth = 1000f;
        MinDepth = 0.1f;
    }

    /// <summary>
    /// Creates a new camera
    /// </summary>
    /// <param name="entity">The parent entity</param>
    /// <param name="cameraType">The type of this camera</param>
    /// <param name="minDepth">The minimum clip distance</param>
    /// <param name="maxDepth">The maximum clip distance</param>
    /// <param name="fovy">The fov (in degrees) of the y axis of the screen</param>
    /// <param name="size">The clip box's X and Y size</param>
    public Camera(Entity entity, CameraType cameraType, float minDepth, float maxDepth, float fovy = 90.0f, Vector2? size = null)
        : base(entity)
    {
        Type = cameraType;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
        Fovy = fovy;
        Size = size ?? Vector2.One;
    }

    /// <summary>
    /// Creates a new camera. (Reccommended use <see cref="Camera(Entity, CameraType, float, float, float, Vector2?)"/>).
    /// This constructor is mostly used for the camera construction in scene files as enums are not supported (yet)
    /// </summary>
    /// <param name="entity">The parent entity</param>
    /// <param name="cameraType">The type of this camera</param>
    /// <param name="minDepth">The minimum clip distance</param>
    /// <param name="maxDepth">The maximum clip distance</param>
    /// <param name="fovy">The fov (in degrees) of the y axis of the screen</param>
    /// <param name="size">The clip box's X and Y size</param>
    public Camera(Entity entity, int cameraType, float minDepth, float maxDepth, float fovy, Vector2 size)
        : base(entity)
    {
        Type = (CameraType)cameraType;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
        Fovy = fovy;
        Size = size;
    }

    /// <summary>
    /// Calculates the View matrix for this camera
    /// </summary>
    /// <returns>The view matrix</returns>
    public virtual Matrix4 GetView()
    {
        Matrix4 view = Matrix4.LookAt(Transform.Position, Transform.Position - Transform.Forwards, Vector3.UnitY);
        return view;
    }

    /// <summary>
    /// Calculates the Projection matrix for this camera
    /// </summary>
    /// <returns>The projection matrix</returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
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
            _ => throw new IndexOutOfRangeException()
        };
    }

    /// <summary>
    /// Renders all given drawables to the screen through the view of this camera
    /// </summary>
    /// <param name="drawables">All the drawables to draw</param>
    public void Render(IDrawable[] drawables)
    {
        StashedView = GetView();
        StashedProjection = GetProjection();

        foreach (var drawable in drawables)
        {
            drawable.Draw(this);
        }
    }
}
