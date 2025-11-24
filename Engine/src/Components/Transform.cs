using Engine.Attributes;
using OpenTK.Mathematics;

namespace Engine.Components;

[ComponentMeta("Transform")]
[DisallowMultiple(true)]
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
            TranslationMatrix = Matrix4.CreateTranslation(_position);
        }
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
            SizeMatrix = Matrix4.CreateScale(_size);
        }
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
            RotationMatrix = Matrix4.CreateRotationZ(MathHelper.DegToRad * _rotation.Z)
                             * Matrix4.CreateRotationY(MathHelper.DegToRad * _rotation.Y)
                             * Matrix4.CreateRotationX(MathHelper.DegToRad * _rotation.X);
            
            Forwards = new Vector3(
                (float)(Math.Cos(MathHelper.DegToRad * _rotation.X) * Math.Sin(MathHelper.DegToRad * _rotation.Y)),
                (float)(-Math.Sin(MathHelper.DegToRad * _rotation.X)),
                (float)(Math.Cos(MathHelper.DegToRad * _rotation.X) * Math.Cos(MathHelper.DegToRad * _rotation.Y)));
            
            Horizontal = new Vector3(Forwards.X, 0, Forwards.Z).Normalized();
            Right = Vector3.Cross(Vector3.UnitY, Forwards);
            Up = Vector3.Cross(Forwards, Right);
        }
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

    public Transform(Entity parent) : base(parent)
    {
        Position = new Vector3(0, 0, 0);
        Size = new Vector3(1, 1, 1);
        Rotation = new Vector3(0, 0, 0);
    }

    public Transform(Entity parent, Vector3 position, Vector3 size, Vector3 rotation) : base(parent)
    {
        Position = position;
        Size = size;
        Rotation = rotation;
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