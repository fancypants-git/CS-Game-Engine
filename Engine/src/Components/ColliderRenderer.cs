using Engine.Attributes;
using Engine.Debugging;
using Engine.Maths;
using JoltPhysicsSharp;
using Mat4 = System.Numerics.Matrix4x4;

namespace Engine.Components;

/// <summary>
/// Renders the vertices and axes of a <see cref="PhysicsObject"/>
/// </summary>
[ComponentMeta("ColliderRenderer")]
[DisallowMultiple]
public class ColliderRenderer : Component
{
    /// <summary>
    /// Creates a new ColliderRenderer
    /// </summary>
    /// <param name="e">The parent entity</param>
    /// <param name="color">The color to draw the vertices in</param>
    /// <param name="wireframe">True if the collider should be rendered in wireframe (reccommended)</param>
    public ColliderRenderer(Entity e, Vector3 color, bool wireframe) : base(e)
    {
        Color = new(color, 255);
        Wireframe = wireframe;
    }

    /// <summary>
    /// The color to render the vertices of the collider in
    /// </summary>
    public JoltColor Color;

    /// <summary>
    /// Whether the collider should be rendered as a wireframe
    /// </summary>
    public bool Wireframe;
    private readonly JoltDebugRenderer Renderer = new();

    public override void Update()
    {
        if (!Entity.GetComponent(out PhysicsObject? po, false)) return;

        var entityTranslation = Mat4.CreateTranslation(po!.Body.CenterOfMassPosition);
        var entityRotation = Mat4.CreateFromQuaternion(po!.Body.Rotation);

        var entityMatrix = entityRotation * entityTranslation;

        // foreach (var c in Entity.GetComponents<Collider>())
        // {
        //     var colliderOffset = Mat4.CreateTranslation(c.Offset);
        //     var colliderRotation = Mat4.CreateFromQuaternion(c.RotationQuat);

        //     var colliderMatrix = colliderRotation * colliderOffset;

        //     var worldMatrix = entityMatrix * colliderMatrix;

        //     c.Shape.Draw(
        //         Renderer,
        //         worldMatrix,
        //         Vector3.One,
        //         Color,
        //         false,
        //         Wireframe
        //     );
        // }

        po.Body.Shape.Draw(Renderer,
                entityMatrix,
                Vector3.One,
                Color,
                false,
                Wireframe
            );


        const float length = 10f;
        Debug.DrawLine(Transform.Position, Transform.Position + (Transform.Forwards * length), System.Drawing.Color.Cyan);
        Debug.DrawLine(Transform.Position, Transform.Position + (Transform.Right * length), System.Drawing.Color.Magenta);
        Debug.DrawLine(Transform.Position, Transform.Position + (Transform.Up * length), System.Drawing.Color.Yellow);
    }

}
