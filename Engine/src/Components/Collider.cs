using Engine.Attributes;
using Engine.Mathematics;
using Engine.Physics;
using OpenTK.Mathematics;

namespace Engine.Components;

[ComponentMeta("Collider")]
public class Collider : Component
{
    protected static List<Collider> AllColliders = [];
    
    public Collider(Entity entity) : base(entity)
    {
        _colliderSize = new Vector3(1);
        _offset = new Vector3(0);
        CalculateBounds();
        
        AllColliders.Add(this);
    }
    
    public Collider(Entity entity, Vector3 scale, Vector3 offset) : base(entity)
    {
        _colliderSize = scale;
        _offset = offset;
        CalculateBounds();
        
        AllColliders.Add(this);
    }
    
    protected Vector3 _colliderSize;
    public Vector3 ColliderSize
    {
        get
        {
            return _colliderSize;
        }
        set
        {
            _colliderSize = value;
            CalculateBounds();
        }
    }
    
    protected Vector3 _offset;
    public Vector3 Offset
    {
        get
        {
            return _offset;
        }
        set
        {
            _offset = value;
            CalculateBounds();
        }
    }
    
    protected Vector3[] Vertices;
    protected Bounds AABB;

    public override void FixedUpdate()
    {
        CalculateBounds();
    }
    
    public virtual void CalculateBounds()
    {
        Vector3 center = Transform.GlobalPosition + _offset;
        Vector3 halfSize = Transform.GlobalSize * _colliderSize / 2f;
        AABB = new Bounds(center - halfSize, center + halfSize);
        Vertices = [
            AABB.Min,
            new Vector3(AABB.Min.X, AABB.Min.Y, AABB.Max.Z),
            new Vector3(AABB.Max.X, AABB.Min.Y, AABB.Min.Z),
            new Vector3(AABB.Max.X, AABB.Min.Y, AABB.Max.Z),
            AABB.Max,
            new Vector3(AABB.Max.X, AABB.Max.Y, AABB.Min.Z),
            new Vector3(AABB.Min.X, AABB.Max.Y, AABB.Max.Z),
            new Vector3(AABB.Min.X, AABB.Max.Y, AABB.Min.Z)
        ];
    }
    
    public virtual CollisionInfo CollidesWithAABB(Collider other)
    {
        bool collided = Entity.Id != other.Entity.Id &&
                        AABB.Min.X < other.AABB.Max.X && other.AABB.Min.X < AABB.Max.X &&
                        AABB.Min.Y < other.AABB.Max.Y && other.AABB.Min.Y < AABB.Max.Y &&
                        AABB.Min.Z < other.AABB.Max.Z && other.AABB.Min.Z < AABB.Max.Z;
        
        return new CollisionInfo(collided);
    }
    
    public virtual CollisionInfo CollidesWith(Collider other)
    {
        if (!CollidesWithAABB(other).Collided) return new CollisionInfo(false);
        
        // TODO use the SAT (Seperating Axis Theorem) to see if the 2 objects collide
        // https://www.geometrictools.com/Documentation/DynamicCollisionDetection.pdf
        
        Vector3[] axes = [
            Transform.Forwards,
            Transform.Right,
            Transform.Up,
            other.Transform.Forwards,
            other.Transform.Right,
            other.Transform.Up,
            Vector3.Cross(Transform.Forwards, other.Transform.Forwards),
            Vector3.Cross(Transform.Forwards, other.Transform.Right),
            Vector3.Cross(Transform.Forwards, other.Transform.Up),
            Vector3.Cross(Transform.Right, other.Transform.Forwards),
            Vector3.Cross(Transform.Right, other.Transform.Right),
            Vector3.Cross(Transform.Right, other.Transform.Up),
            Vector3.Cross(Transform.Up, other.Transform.Forwards),
            Vector3.Cross(Transform.Up, other.Transform.Right),
            Vector3.Cross(Transform.Up, other.Transform.Up),
        ];
        
        bool collided = true;
        foreach (Vector3 axis in axes)
        {
            if (axis.Length <= 1e-5)
                continue;
            
            axis.Normalize();
            
            Vector3 direction = other.Transform.GlobalPosition - Transform.GlobalPosition;
            float projectionAlignment = Math.Abs(Vector3.Dot(direction, axis));
            
            float radius0 = Math.Abs(Vector3.Dot(Transform.Forwards * _colliderSize, axis)) +
                           Math.Abs(Vector3.Dot(Transform.Right * _colliderSize, axis)) +
                           Math.Abs(Vector3.Dot(Transform.Up * _colliderSize, axis));
                           
            float radius1 = Math.Abs(Vector3.Dot(other.Transform.Forwards * other._colliderSize, axis)) +
                           Math.Abs(Vector3.Dot(other.Transform.Right * other._colliderSize, axis)) +
                           Math.Abs(Vector3.Dot(other.Transform.Up * other._colliderSize, axis));
                           
            float overlap = (radius0 + radius1) - projectionAlignment;
            
            if (overlap < 0)
            {
                collided = false;
                break;
            }
        }
        
        return new CollisionInfo(collided);
    }
    
    
    public virtual CollisionInfo[] CalculateCollisions()
    {
        List<CollisionInfo> collisions = [];
        
        foreach (Collider other in AllColliders)
        {
            if (other.Entity.Id == Entity.Id) continue;
            
            CollisionInfo collision = CollidesWith(other);
            
            if (collision.Collided) collisions.Add(collision);
        }
        
        return collisions.ToArray();
    }
}
