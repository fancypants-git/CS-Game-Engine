using Engine.Attributes;
using Engine.Helpers;
using Engine.Maths;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine.Components;

[ComponentMeta("PlayerController")]
[DisallowMultiple]
public class PlayerController : Component
{
    public float Speed;

    public PlayerController(Entity entity, float speed) : base(entity)
    {
        Speed = speed;
    }

    public override void Update()
    {
        base.Update();
        
        float toMove = Speed * Time.DeltaTime;

        if (Input.IsKeyDown(Keys.W))
            Transform.Translate(-Vector3.UnitZ, toMove, Space.Local);
        if (Input.IsKeyDown(Keys.S))
            Transform.Translate(Vector3.UnitZ, toMove, Space.Local);
        if (Input.IsKeyDown(Keys.D))
            Transform.Translate(Vector3.UnitX, toMove, Space.Local);
        if (Input.IsKeyDown(Keys.A))
            Transform.Translate(-Vector3.UnitX, toMove, Space.Local);
        if (Input.IsKeyDown(Keys.Space))
            Transform.Translate(Vector3.UnitY, toMove, Space.World);
        if (Input.IsKeyDown(Keys.LeftShift))
            Transform.Translate(-Vector3.UnitY, toMove, Space.World);
    }
}
