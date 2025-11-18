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
        
        Debug.Log("DeltaTime:", Time.DeltaTime);
        
        
        if (Input.IsKeyDown(Keys.W))
            SceneManager.ActiveCamera.Transform.Translate(Speed * Time.DeltaTime, SceneManager.ActiveCamera.Transform.Forwards);
        if (Input.IsKeyDown(Keys.S))
            SceneManager.ActiveCamera.Transform.Translate(Speed * Time.DeltaTime, -SceneManager.ActiveCamera.Transform.Forwards);
        if (Input.IsKeyDown(Keys.D))
            SceneManager.ActiveCamera.Transform.Translate(Speed * Time.DeltaTime, -SceneManager.ActiveCamera.Transform.Right);
        if (Input.IsKeyDown(Keys.A))
            SceneManager.ActiveCamera.Transform.Translate(Speed * Time.DeltaTime, SceneManager.ActiveCamera.Transform.Right);
        if (Input.IsKeyDown(Keys.Space))
            SceneManager.ActiveCamera.Transform.Translate(Speed * Time.DeltaTime, SceneManager.ActiveCamera.Transform.Up);
            
        if (Input.IsKeyDown(Keys.LeftShift))
            SceneManager.ActiveCamera.Transform.Translate(Speed * Time.DeltaTime, -SceneManager.ActiveCamera.Transform.Up);
    }
}