using Engine.Attributes;
using OpenTK.Mathematics;

namespace Engine.Components;

[ComponentMeta("Transform")]
[DisallowMultiple]
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
            TranslationMatrix = Matrix4.CreateTranslation(GlobalPosition);
        }
    }

    public Vector3 GlobalPosition
    {
        get => _position + (Parent?._position ?? Vector3.AdditiveIdentity);
        set => Position = value - (Parent?._position ?? Vector3.AdditiveIdentity);
    }
    

    private Vector3 _size;
    public Vector3 Size
    {
        get
        {
            return _size;
        }
        set
        {
            _size = value;
            SizeMatrix = Matrix4.CreateScale(GlobalSize);
        }
    }

    public Vector3 GlobalSize
    {
        get => _size * (Parent?._size ?? Vector3.MultiplicativeIdentity);
        set => Size = value / (Parent?._size ?? Vector3.MultiplicativeIdentity);
    }

    
    private Vector3 _rotation;
    public Vector3 Rotation
    {
        get
        {
            return _rotation;
        }
        set
        {
            _rotation = value;
            RotationMatrix = Matrix4.CreateRotationZ(MathHelper.DegToRad * GlobalRotation.Z)
                             * Matrix4.CreateRotationY(MathHelper.DegToRad * GlobalRotation.Y)
                             * Matrix4.CreateRotationX(MathHelper.DegToRad * GlobalRotation.X);
            
            Forwards = new Vector3(
                (float)(Math.Cos(MathHelper.DegToRad * _rotation.X) * Math.Sin(MathHelper.DegToRad * _rotation.Y)),
                (float)(-Math.Sin(MathHelper.DegToRad * _rotation.X)),
                (float)(Math.Cos(MathHelper.DegToRad * _rotation.X) * Math.Cos(MathHelper.DegToRad * _rotation.Y)));
            
            Horizontal = new Vector3(Forwards.X, 0, Forwards.Z).Normalized();
            Right = Vector3.Cross(Vector3.UnitY, Forwards);
            Up = Vector3.Cross(Forwards, Right);
        }
    }

    public Vector3 GlobalRotation
    {
        get => _rotation + (Parent?._rotation ?? Vector3.AdditiveIdentity);
        set => Rotation = value - (Parent?._rotation ?? Vector3.AdditiveIdentity);
    }


    private Matrix4 _translationMatrix;
    public Matrix4 TranslationMatrix
    {
        get
        {
            return _translationMatrix;
        }
        private set
        {
            _translationMatrix = value;
            SetModelMatrix();
        }
    }
    
    private Matrix4 _sizeMatrix;
    public Matrix4 SizeMatrix
    {
        get
        {
            return _sizeMatrix;
        }
        private set
        {
            _sizeMatrix = value;
            SetModelMatrix();
        }
    }
    
    private Matrix4 _rotationMatrix;
    public Matrix4 RotationMatrix
    {
        get
        {
            return _rotationMatrix;
        }
        private set
        {
            _rotationMatrix = value;
            SetModelMatrix();
        }
    }
    
    public Matrix4 ModelMatrix { get; private set; }
    
    public Vector3 Forwards { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Up { get; private set; }
    public Vector3 Horizontal { get; private set; }

    private Transform? _parent;

    public Transform? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
        }
    }

    public Transform(Entity entity) : base(entity)
    {
        Position = new Vector3(0, 0, 0);
        Size = new Vector3(1, 1, 1);
        Rotation = new Vector3(0, 0, 0);
    }

    public Transform(Entity entity, Vector3 position, Vector3 size, Vector3 rotation) : base(entity)
    {
        Position = position;
        Size = size;
        Rotation = rotation;
    }

    public Transform(Entity entity, Transform parent) : base(entity)
    {
        Position = new Vector3(0, 0, 0);
        Size = new Vector3(1, 1, 1);
        Rotation = new Vector3(0, 0, 0);
        Parent = parent;
    }

    public Transform(Entity entity, Transform parent, Vector3 position, Vector3 size, Vector3 rotation)
        : base(entity)
    {
        Position = position;
        Size = size;
        Rotation = rotation;
        Parent = parent;
    }
    
    private void SetModelMatrix()
    {
        ModelMatrix = _translationMatrix * _sizeMatrix * _rotationMatrix;
    }
    
    
    // TRANSFORMATIONS

    public void Translate(Vector3 v)
    {
        Position += v;
    }
    public void Translate(float x, float y, float z)
    {
        Position += new Vector3(x, y, z);
    }
    public void Translate(float v, Vector3 d)
    {
        Position += d * v;
    }
    
    public void Scale(Vector3 v)
    {
        Size *= v;
    }
    public void Scale(float x, float y, float z)
    {
        Size *= new Vector3(x, y, z);
    }

    public void Rotate(Vector3 v)
    {
        Rotation += v;
    }
    public void Rotate(float x, float y, float z)
    {
        Rotation += new Vector3(x, y, z);
    }
    public void Rotate(float v, Vector3 d)
    {
        Rotation += d * v;
    }

    public void RotateClamped(Vector3 v, Vector3 min, Vector3 max)
    {
        Rotation = Vector3.Clamp(_rotation + v, min, max);
    }
    public void RotateClamped(float x, float y, float z, Vector3 min, Vector3 max)
    {
        Rotation = Vector3.Clamp(_rotation + new Vector3(x, y, z), min, max);
    }
    public void RotateClamped(float v, Vector3 d, Vector3 min, Vector3 max)
    {
        Rotation = Vector3.Clamp(_rotation + (v * d), min, max);
    }

    public void RotateXClamped(float v, float min, float max)
    {
        float clampedX = Math.Clamp(_rotation.X + v, min, max);
        Rotation = new Vector3(clampedX, _rotation.Y, _rotation.Z);
    }
    public void RotateYClamped(float v, float min, float max)
    {
        float clampedY = Math.Clamp(_rotation.Y + v, min, max);
        Rotation = new Vector3(_rotation.X, clampedY, _rotation.Z);
    }
    public void RotateZClamped(float v, float min, float max)
    {
        float clampedZ = Math.Clamp(_rotation.Z + v, min, max);
        Rotation = new Vector3(_rotation.X, _rotation.Y, clampedZ);
    }
}