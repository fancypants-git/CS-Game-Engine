using System.Drawing;
using Engine.Attributes;
using OpenTK.Mathematics;
using Engine.Helpers;
using Engine.Mathematics;
using Engine.Physics;

namespace Engine.Components;

[ComponentMeta("BoxCollider")]
public class BoxCollider : Collider
{
    private Vector3 _colliderSize;
    public Vector3 ColliderSize
    {
        get
        {
            return _colliderSize;
        }
        set
        {
            _colliderSize = value;
            CalculateAABB();
        }
    }
    
    private Vector3 _offset;
    public Vector3 Offset
    {
        get
        {
            return _offset;
        }
        set
        {
            _offset = value;
            CalculateAABB();
        }
    }
    
    
    public Bounds AABB { get; private set; }
    
    public BoxCollider(Entity entity) : base(entity) {}
    
    public BoxCollider(Entity entity, Vector3 size, Vector3 offset) : base(entity)
    {
        _colliderSize = size;
        _offset = offset;
        CalculateAABB();
    }
    
    public void CalculateAABB()
    {
        Vector3 center = Transform.GlobalPosition + _offset;
        Vector3 totalHalfSize = Transform.GlobalSize * _colliderSize / 2;
        Vector3 min = center - totalHalfSize;
        Vector3 max = center + totalHalfSize;
        AABB = new Bounds(min, max);
    }
    
    public CollisionInfo CollidesWithAABB(BoxCollider other)
    {
        bool collided = Entity.Id != other.Entity.Id &&
                        AABB.Min.X < other.AABB.Max.X && other.AABB.Min.X < AABB.Max.X &&
                        AABB.Min.Y < other.AABB.Max.Y && other.AABB.Min.Y < AABB.Max.Y &&
                        AABB.Min.Z < other.AABB.Max.Z && other.AABB.Min.Z < AABB.Max.Z;
        
        return new CollisionInfo(collided);
    }
    
    public CollisionInfo[] CollidesWithAny()
    {
        // TODO: create this method
        throw new NotImplementedException();
    }
}
