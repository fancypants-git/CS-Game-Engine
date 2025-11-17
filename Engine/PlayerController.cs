namespace Engine;

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
        
        // TODO implement Time.deltaTime etc.
    }
}