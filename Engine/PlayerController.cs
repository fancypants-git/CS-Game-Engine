using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Engine;

[ComponentMeta("PlayerController")]
[DisallowMultiple(true)]
public class PlayerController : Component
{
    public float Speed;

    public PlayerController(Entity parent, float speed) : base(parent)
    {
        Speed = speed;
    }

    public override void Update()
    {
        base.Update();
        
        if (Input.IsKeyDown(Keys.W))
            Transform.Translate(Speed * Time.DeltaTime, Transform.Forwards);
        if (Input.IsKeyDown(Keys.S))
            Transform.Translate(Speed * Time.DeltaTime, -Transform.Forwards);
        if (Input.IsKeyDown(Keys.D))
            Transform.Translate(Speed * Time.DeltaTime, -Transform.Right);
        if (Input.IsKeyDown(Keys.A))
            Transform.Translate(Speed * Time.DeltaTime, Transform.Right);
        if (Input.IsKeyDown(Keys.Space))
            Transform.Translate(Speed * Time.DeltaTime, Transform.Up);
        if (Input.IsKeyDown(Keys.LeftShift))
            Transform.Translate(Speed * Time.DeltaTime, -Transform.Up);
    }
}