using Engine.Attributes;
using Engine.Physics;
using Engine.Debugging;
using OpenTK.Mathematics;
using Engine.Helpers;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
        
        BoundingVolumeHierarchy = new BVH(r.Mesh.Vertices.Select(v => v.Position).ToArray(), r.Mesh.Indices);
        _colliders.Add(this);
    }
    
    /// <summary>
    /// Creates a new Collider with a BVH that is created from the given vertices and indices.
    /// </summary>
    /// <param name="entity">Entity that this Collider is attached to</param>
    /// <param name="vertices">The vertices for the BVH</param>
    /// <param name="indices">The indices for the BVH triangles</param>
    public Collider(Entity entity, Vector3[] vertices, uint[] indices) : base(entity)
    {
        BoundingVolumeHierarchy = new BVH(vertices, indices);
        _colliders.Add(this);
    }
    
    protected BVH BoundingVolumeHierarchy;
    private int _renderDepth = 0;
    
    public CollisionInfo CollidesWith(Collider other)
    {
        if (Entity.Id == other.Entity.Id) return CollisionInfo.NoCollision;
        
        return BVHTreeNode.Collide(BoundingVolumeHierarchy.Root, Transform.ModelMatrix, other.BoundingVolumeHierarchy.Root, other.Transform.ModelMatrix);
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

    public override void Update()
    {
        if (Input.IsKeyPressed(Keys.Up))
            _renderDepth ++;
        if (Input.IsKeyPressed(Keys.Down))
            _renderDepth --;
        
        BoundingVolumeHierarchy.Root.DrawNodeAndChildrenBounds(Transform.ModelMatrix, _renderDepth);
    }

    public override void FixedUpdate()
    {
    }
}
