namespace Engine.Maths;

public struct Vector2
{
    public Vector2(float scalar)
    {
        X = scalar;
        Y = scalar;
    }
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }
    public Vector2(Vector2 v)
    {
        X = v.X;
        Y = v.Y;
    }
    
    public float X, Y;
    
    public readonly float LengthSquared => X * X + Y * Y;
    public readonly float Length => MathF.Sqrt(LengthSquared);
    
    public static Vector2 UnitX => new(1, 0);
    public static Vector2 UnitY => new(0, 1);
    public static Vector2 Zero => new(0);
    public static Vector2 One => new(1);
    public static Vector2 MultiplicativeIdentity => new(1);
    public static Vector2 AdditiveIdentity => new(0);
    
    public readonly Vector2 Normalized()
    {
        var length = Length;
        if (length == 0)
            return new Vector2(0);
        return new Vector2(X / length, Y / length);
    }
    public void Normalize()
    {
        X /= Length;
        Y /= Length;
    }
    public static Vector2 Normalize(Vector2 vec)
    {
        return new(vec.X / vec.Length, vec.Y / vec.Length);
    }
    
    public readonly Vector2 Clamped(Vector2 min, Vector2 max)
    {
        float x = X < min.X ? min.X : X > max.X ? max.X : X;
        float y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
        return new(x, y);
    }
    public void Clamp(Vector2 min, Vector2 max)
    {
        X = X < min.X ? min.X : X > max.X ? max.X : X;
        Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
    }
    
    public static Vector2 Clamp(Vector2 vec, Vector2 min, Vector2 max)
    {
        float x = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
        float y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
        return new(x, y);
    }
    public static float Cross(Vector2 a, Vector2 b)
    {
        return a.Length * b.Length * (float)Math.Sin(Angle(a, b));
    }
    
    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }
    
    public static float Angle(Vector2 a, Vector2 b)
    {
        return (float)Math.Atan2(b.X - a.X, b.Y - a.Y);
    }
    
    public float this[int index]
    {
        readonly get
        {
            return index switch
            {
                0 => X,
                1 => Y,
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
            }
        }
    }
    
    public static implicit operator System.Numerics.Vector2(Vector2 v)
        => new(v.X, v.Y);
    public static implicit operator OpenTK.Mathematics.Vector2(Vector2 v)
        => new(v.X, v.Y);
    public static explicit operator OpenTK.Mathematics.Vector2i(Vector2 v)
        => new((int)v.X, (int)v.Y);
        
    public static implicit operator Vector2(OpenTK.Mathematics.Vector2 v)
        => new(v.X, v.Y);
    public static implicit operator Vector2(System.Numerics.Vector2 v)
        => new(v.X, v.Y);
    public static implicit operator Vector2(OpenTK.Mathematics.Vector2i v)
        => new(v.X, v.Y);
        
    public static Vector2 operator +(Vector2 a)
        => a;
    public static Vector2 operator -(Vector2 a)
        => new(-a.X, -a.Y);
        
    public static Vector2 operator +(Vector2 a, Vector2 b)
        => new(a.X + b.X, a.Y + b.Y);
    public static Vector2 operator -(Vector2 a, Vector2 b)
        => new(a.X - b.X, a.Y - b.Y);
    public static Vector2 operator *(Vector2 a, Vector2 b)
        => new(a.X * b.X, a.Y * b.Y);
    public static Vector2 operator /(Vector2 a, Vector2 b)
        => new(a.X / b.X, a.Y / b.Y);
        
    public static Vector2 operator +(Vector2 a, float b)
        => new(a.X + b, a.Y + b);
    public static Vector2 operator -(Vector2 a, float b)
        => new(a.X - b, a.Y - b);
    public static Vector2 operator *(Vector2 a, float b)
        => new(a.X * b, a.Y * b);
    public static Vector2 operator /(Vector2 a, float b)
        => new(a.X / b, a.Y / b);
        
    public static Vector2 operator ++(Vector2 a)
        => new(a.X++, a.Y++);
    public static Vector2 operator --(Vector2 a)
        => new(a.X--, a.Y--);
        
    public readonly override string ToString()
        => $"<{X}, {Y}>";
        
    public void operator +=(Vector2 a)
    {
        X += a.X;
        Y += a.Y;
    }
    public void operator -=(Vector2 a)
    {
        X -= a.X;
        Y -= a.Y;
    }
    public void operator *=(Vector2 a)
    {
        X *= a.X;
        Y *= a.Y;
    }
    public void operator /=(Vector2 a)
    {
        if (a.X == 0 || a.Y == 0)
            throw new DivideByZeroException();
        X /= a.X;
        Y /= a.Y;
    }
    
    public void operator +=(float a)
    {
        X += a;
        Y += a;
    }
    public void operator -=(float a)
    {
        X -= a;
        Y -= a;
    }
    public void operator *=(float a)
    {
        X *= a;
        Y *= a;
    }
    public void operator /=(float a)
    {
        if (a == 0)
            throw new DivideByZeroException();
        X /= a;
        Y /= a;
    }
}
