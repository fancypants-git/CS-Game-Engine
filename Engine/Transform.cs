using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;

namespace Engine;

public class Transform : Component
{
    private Vector3 _position;
    public Vector3 Position
    {
        get
        {
            return _position;
        }
        set
        {
            _position = value;
            TransformMatrix = Matrix4.CreateTranslation(_position);
        }
    }
    
    
    private Matrix4 _transformMatrix;

    public Matrix4 TransformMatrix
    {
        get
        {
            return _transformMatrix;
        }
        private set
        {
            _transformMatrix = value;
            SetModelMatrix();
        }
    }
    
    public Matrix4 ModelMatrix { get; private set; }
    

    public Transform(Entity parent) : base(parent)
    {
        Position = new Vector3(0, 0, 0);
    }

    public Transform(Vector3 position, Entity parent) : base(parent)
    {
        Position = position;
    }


    private void SetModelMatrix()
    {
        ModelMatrix = TransformMatrix;
    }
}