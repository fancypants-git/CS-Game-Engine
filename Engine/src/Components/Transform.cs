using Engine.Attributes;
using Engine.Maths;
using Matrix4 = OpenTK.Mathematics.Matrix4;
using Quaternion = OpenTK.Mathematics.Quaternion;

namespace Engine.Components;

[ComponentMeta("Transform")]
[DisallowMultiple]
public class Transform : Component
{
    public Transform(Entity e, Transform parent) : base(e)
    {
        Parent = parent;
        LocalPosition = Vector3.Zero;
        LocalRotation = Quaternion.Identity;
        LocalSize = Vector3.One;
    }
    public Transform(Entity e, Vector3 position, Vector3 rotation, Vector3 size)
        : base(e)
    {
        Parent = null!;
        LocalPosition = position;
        LocalRotation = Quaternion.FromEulerAngles(rotation * MathHelper.DegToRad);
        LocalSize = size;
    }
    public Transform(Entity e, Transform parent, Vector3 position, Vector3 rotation, Vector3 size)
        : base(e)
    {
        Parent = parent;
        LocalPosition = position;
        LocalRotation = Quaternion.FromEulerAngles(rotation * MathHelper.DegToRad);
        LocalSize = size;
    }


    #region Properties
    protected Vector3 _localPosition;
    public Vector3 LocalPosition
    {
        get => _localPosition;
        set => InternalUpdatePosition(value, Space.Local);
    }

    public Vector3 Position
    {
        get
        {
            if (Parent == null)
                return _localPosition;
            
            return Parent.Position + (Vector3)(Parent.Rotation * _localPosition);
        }
        set => InternalUpdatePosition(value, Space.World);
    }



    protected Vector3 _localSize;
    public Vector3 LocalSize
    {
        get => _localSize;
        set => InternalUpdateSize(value, Space.Local);
    }

    public Vector3 Size
    {
        get => _localSize * (Parent?.Size ?? Vector3.MultiplicativeIdentity);
        set => InternalUpdateSize(value, Space.World);
    }



    protected Quaternion _localRotation;
    public Quaternion LocalRotation
    {
        get => _localRotation;
        set => InternalUpdateRotation(value, Space.Local);
    }

    public Quaternion Rotation
    {
        get => (Parent?.Rotation ?? Quaternion.Identity) * _localRotation;
        set => InternalUpdateRotation(value, Space.World);
    }



    protected Transform? _parent;
    public Transform? Parent
    {
        get => _parent;
        set => _parent = value;
    }



    public Vector3 Forwards { get; protected set; }
    public Vector3 Right { get; protected set; }
    public Vector3 Up { get; protected set; }

    protected Matrix4 _translationMatrix;
    public Matrix4 TranslationMatrix
    {
        get
        {
            if (_translationDirty)
                InternalUpdateTranslationMatrix();
            
            return _translationMatrix;
        }
    }

    protected Matrix4 _sizeMatrix;
    public Matrix4 SizeMatrix
    {
        get
        {
            if (_sizeDirty)
                InternalUpdateSizeMatrix();

            return _sizeMatrix;
        }
    }

    protected Matrix4 _rotationMatrix;
    public Matrix4 RotationMatrix
    {
        get
        {
            if (_rotationDirty)
                InternalUpdateRotationMatrix();
            
            return _rotationMatrix;
        }
    }

    protected Matrix4 _modelMatrix;
    public Matrix4 ModelMatrix
    {
        get
        {
            if (_modelDirty)
                InternalUpdateModelMatrix();

            return _modelMatrix;
        }
    }

    protected bool _translationDirty;
    protected bool _rotationDirty;
    protected bool _sizeDirty;
    protected bool _modelDirty;
    #endregion


    #region Internal Updates
    protected void InternalUpdatePosition(Vector3 value, Space space)
    {
        if (space == Space.Local || Parent == null)
        {
            _localPosition = value;
        }
        else if (space == Space.World)
        {
            Vector3 delta = value - Parent.Position;
            Quaternion invParentRotation = Quaternion.Conjugate(Parent.Rotation);
            _localPosition = invParentRotation * delta;
        }

        _translationDirty = true;
        _modelDirty = true;
    }

    protected void InternalUpdateSize(Vector3 value, Space space)
    {
        if (space == Space.Local || Parent == null)
        {
            _localSize = value;
        }
        else if (space == Space.World)
        {
            _localSize = value / Parent.Size;
        }

        _sizeDirty = true;
        _modelDirty = true;
    }

    protected void InternalUpdateRotation(Quaternion value, Space space)
    {
        if (space == Space.Local || Parent == null)
        {
            _localRotation = value;
        }
        else if (space == Space.World)
        {
            // given Quaternions P, Q and R
            // with P and Q being a spacial rotation
            // and R = P * Q
            // gives R = P( Q )
            // 
            // P is Parent.Rotation
            // Q is localRotation
            // R is result is globalRotation
            Quaternion p = Parent.Rotation;
            Quaternion pInvert = Quaternion.Conjugate(p);
            _localRotation = pInvert * value;
        }

        _localRotation.Normalize();

        Forwards = Rotation * Vector3.UnitZ;
        Right = Rotation * Vector3.UnitX;
        Up = Rotation * Vector3.UnitY;

        _rotationDirty = true;
        _modelDirty = true;
    }

    protected void InternalUpdateTranslationMatrix()
    {
        _translationMatrix = Matrix4.CreateTranslation(Position);
        _translationDirty = false;
        _modelDirty = true;
    }

    protected void InternalUpdateRotationMatrix()
    {
        _rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
        _rotationDirty = false;
        _modelDirty = true;
    }

    protected void InternalUpdateSizeMatrix()
    {
        _sizeMatrix = Matrix4.CreateScale(Size / 2f);
        _sizeDirty = false;
        _modelDirty = true;
    }

    protected void InternalUpdateModelMatrix()
    {
        _modelMatrix = SizeMatrix * RotationMatrix * TranslationMatrix;
        _modelDirty = false;
    }
    #endregion

    #region Transformations

    public void Translate(Vector3 v, Space space = Space.World)
    {
        if (space == Space.Local)
        {
            v = Rotation * v;
        }

        InternalUpdatePosition(_localPosition + v, Space.Local);
    }

    public void Translate(float x, float y, float z, Space space = Space.World)
    {
        Translate(new(x, y, z), space);
    }

    public void Translate(Vector3 d, float l, Space space = Space.World)
    {
        Translate(d * l, space);
    }


    public void Rotate(Quaternion delta, Space space = Space.World)
    {
        // World rotation
        if (Parent != null)
        {
            Quaternion pInv = Quaternion.Conjugate(Parent.Rotation);
            delta = pInv * delta * Parent.Rotation;
        }

        InternalUpdateRotation(Quaternion.Normalize(_localRotation * delta), Space.Local);
    }

    // angle in radians!!!
    public void Rotate(Vector3 axis, float angle, Space space = Space.World)
    {
        Quaternion quat = Quaternion.FromAxisAngle(axis, angle);
        Rotate(quat, space);
    }

    #endregion
}
