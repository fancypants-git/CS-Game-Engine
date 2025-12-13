using Engine.Attributes;
using Engine.Interfaces;
using Engine.Physics;
using Engine.Helpers;
using OpenTK.Mathematics;

namespace Engine.Components;

[ComponentMeta("Collider")]
public class Collider : Component
{
    public static List<Collider> _colliders = [];
    /// <summary>
    /// Creates a new Collider with a BVH that is created from the Mesh stored in a
    /// Renderer Component attached to the parent Entity
    /// </summary>
    /// <param name="entity">Entity that this Collider is attached to</param>
    public Collider(Entity entity) : base(entity)
    {
        if (!Entity.GetComponent(out Renderer r, true))
        {
            Debug.Log("Entity Does not contain Renderer! Can't create Collision BVH.");
            return;
        }
        
        BoundingVolumeHierarchy = new BVH(r.Mesh.Vertices.Select(v => v.Position).ToArray(), r.Mesh.Indices, true, Transform.ModelMatrix);
        _colliders.Add(this);
    }
    
    /// <summary>
    /// Creates a new Collider with a BVH that is created from the given vertices and indices.
    /// </summary>
    /// <param name="entity">Entity that this Collider is attached to</param>
    /// <param name="vertices">The vertices for the BVH</param>
    /// <param name="indices">The indices for the BVH triangles</param>
    public Collider(Entity entity, Vector3[] vertices, uint[] indices, bool normalized) : base(entity)
    {
        BoundingVolumeHierarchy = new BVH(vertices, indices, normalized, Transform.ModelMatrix);
        _colliders.Add(this);
    }
    
    protected BVH BoundingVolumeHierarchy;
    
    public CollisionInfo CollidesWith(Collider other)
    {
        if (Entity.Id == other.Entity.Id) return new CollisionInfo();
        
        
    }
    
    public CollisionInfo[] CollidesWithAny()
    {
        List<CollisionInfo> collisions = [];
        foreach (Collider other in _colliders)
        {
            CollisionInfo collision = CollidesWith(other);
            if (collision.Collided)
                collisions.Add(collision);
        }
        
        return collisions.ToArray();
    }
}
