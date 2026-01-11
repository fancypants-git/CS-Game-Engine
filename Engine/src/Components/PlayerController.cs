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

        // Input? input = Application.WindowManager.GetMainWindow()?.InputHandler;

        // if (input == null)
        //     return;

        // if (input.IsKeyDown(Keys.W))
        //     Transform.Translate(Vector3.UnitZ, toMove, Space.Local);
        // if (input.IsKeyDown(Keys.S))
        //     Transform.Translate(-Vector3.UnitZ, toMove, Space.Local);
        // if (input.IsKeyDown(Keys.D))
        //     Transform.Translate(-Vector3.UnitX, toMove, Space.Local);
        // if (input.IsKeyDown(Keys.A))
        //     Transform.Translate(Vector3.UnitX, toMove, Space.Local);
        // if (input.IsKeyDown(Keys.Space))
        //     Transform.Translate(Vector3.UnitY, toMove, Space.World);
        // if (input.IsKeyDown(Keys.LeftShift))
        //     Transform.Translate(-Vector3.UnitY, toMove, Space.World);
    }
}
