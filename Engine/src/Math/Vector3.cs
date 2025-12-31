namespace Engine.Maths;

public struct Vector3
{
    public Vector3(float scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
    }
    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public Vector3(float x, float y)
    {
        X = x;
        Y = y;
        Z = 0;
    }
    public Vector3(Vector3 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }
    
    public float X, Y, Z;
    
    public readonly float LengthSquared => X*X + Y*Y + Z*Z;
    public readonly float Length => MathF.Sqrt(LengthSquared);
    
    public static Vector3 UnitX => new(1, 0, 0);
    public static Vector3 UnitY => new(0, 1, 0);
    public static Vector3 UnitZ => new(0, 0, 1);
    public static Vector3 Zero => new(0);
    public static Vector3 One => new(1);
    public static Vector3 MultiplicativeIdentity => new(1);
    public static Vector3 AdditiveIdentity => new(0);
    
    
    public readonly Vector3 Normalized()
    {
        return new(X / Length, Y / Length, Z / Length);
    }
    public void Normalize()
    {
        X /= Length;
        Y /= Length;
        Z /= Length;
    }
    public static Vector3 Normalize(Vector3 vec)
    {
        return vec.Normalized();
    }
    
    public readonly Vector3 Clamped(Vector3 min, Vector3 max)
    {
        float x = X < min.X ? min.X : X > max.X ? max.X : X;
        float y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
        float z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
        return new(x, y, z);
    }
    public void Clamp(Vector3 min, Vector3 max)
    {
        X = X < min.X ? min.X : X > max.X ? max.X : X;
        Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
        Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
    }
    
    public static Vector3 Clamp(Vector3 vec, Vector3 min, Vector3 max)
    {
        float x = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
        float y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
        float z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
        return new(x, y, z);
    }
    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        return new(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
    }
    
    public static float Dot(Vector3 a, Vector3 b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }
    
    public static float Angle(Vector3 a, Vector3 b)
    {
        return (float)Math.Acos(Dot(a, b) / (a.Length * b.Length));
    }
    
    
    public float this[int index]
    {
        readonly get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => 0
            };
        }
        set
        {
            switch(index)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Y = value;
                    break;
                case 2:
                    Z = value;
                    break;
            }
        }
    }
    
    public static implicit operator System.Numerics.Vector3(Vector3 v)
        => new(v.X, v.Y, v.Z);
    public static implicit operator OpenTK.Mathematics.Vector3(Vector3 v)
        => new(v.X, v.Y, v.Z);
    public static explicit operator OpenTK.Mathematics.Vector3i(Vector3 v)
        => new((int)v.X, (int)v.Y, (int)v.Z);
        
    public static implicit operator Vector3(System.Numerics.Vector3 v)
        => new(v.X, v.Y, v.Z);
    public static implicit operator Vector3(OpenTK.Mathematics.Vector3 v)
        => new(v.X, v.Y, v.Z);
    public static implicit operator Vector3(OpenTK.Mathematics.Vector3i v)
        => new(v.X, v.Y, v.Z);
    
    public static Vector3 operator +(Vector3 a)
        => a;
    public static Vector3 operator -(Vector3 a)
        => new(-a.X, -a.Y, -a.Z);
        
    public static Vector3 operator +(Vector3 a, Vector3 b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static Vector3 operator -(Vector3 a, Vector3 b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static Vector3 operator *(Vector3 a, Vector3 b)
        => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
    public static Vector3 operator /(Vector3 a, Vector3 b)
        => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        
    public static Vector3 operator +(Vector3 a, float b)
        => new(a.X + b, a.Y + b, a.Z + b);
    public static Vector3 operator -(Vector3 a, float b)
        => new(a.X - b, a.Y - b, a.Z - b);
    public static Vector3 operator *(Vector3 a, float b)
        => new(a.X * b, a.Y * b, a.Z * b);
    public static Vector3 operator /(Vector3 a, float b)
        => new(a.X / b, a.Y / b, a.Z / b);
        
    public static Vector3 operator +(float a, Vector3 b)
        => new(a + b.X, a + b.Y, a + b.Z);
    public static Vector3 operator -(float a, Vector3 b)
        => new(a - b.X, a - b.Y, a - b.Z);
    public static Vector3 operator *(float a, Vector3 b)
        => new(a * b.X, a * b.Y, a * b.Z);
    public static Vector3 operator /(float a, Vector3 b)
        => new(a / b.X, a / b.Y, a / b.Z);
        
    public static Vector3 operator ++(Vector3 a)
        => new(a.X++, a.Y++, a.Z++);
    public static Vector3 operator --(Vector3 a)
        => new(a.X--, a.Y--, a.Z--);
        
    public readonly override string ToString()
        => $"<{X}, {Y}, {Z}>";
        
    public void operator +=(Vector3 a)
    {
        X += a.X;
        Y += a.Y;
        Z += a.Z;
    }
    public void operator -=(Vector3 a)
    {
        X -= a.X;
        Y -= a.Y;
        Z -= a.Z;
    }
    public void operator *=(Vector3 a)
    {
        X *= a.X;
        Y *= a.Y;
        Z *= a.Z;
    }
    public void operator /=(Vector3 a)
    {
        if (a.X == 0 || a.Y == 0 || a.Z == 0)
            throw new DivideByZeroException();
        X /= a.X;
        Y /= a.Y;
        Z /= a.Z;
    }
    
    public void operator +=(float a)
    {
        X += a;
        Y += a;
        Z += a;
    }
    public void operator -=(float a)
    {
        X -= a;
        Y -= a;
        Z -= a;
    }
    public void operator *=(float a)
    {
        X *= a;
        Y *= a;
        Z *= a;
    }
    public void operator /=(float a)
    {
        if (a == 0)
            throw new DivideByZeroException();
        X /= a;
        Y /= a;
        Z /= a;
    }
}
