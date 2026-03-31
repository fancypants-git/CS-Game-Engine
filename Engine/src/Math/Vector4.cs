namespace Engine.Maths;

public struct Vector4
{
    public Vector4(float scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
        W = scalar;
    }
    public Vector4(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
    public Vector4(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
        W = 1;
    }


    public float X, Y, Z, W;

    public readonly float LengthSquared => X*X + Y*Y + Z*Z + W*W;
    public readonly float Length => MathF.Sqrt(LengthSquared);

    public static Vector4 UnitX => new(1, 0, 0, 0);
    public static Vector4 UnitY => new(0, 1, 0, 0);
    public static Vector4 UnitZ => new(0, 0, 1, 0);
    public static Vector4 UnitW => new(0, 0, 0, 1);
    public static Vector4 Zero => new(0);
    public static Vector4 One => new(1);
    public static Vector4 MultiplicativeIdentity => new(1);
    public static Vector4 AdditiveIdentity => new(0);

    public readonly Vector4 Normalized()
    {
        return new(X / Length, Y / Length, Z / Length, W / Length);
    }
    public void Normalize()
    {
        X /= Length;
        Y /= Length;
        Z /= Length;
        W /= Length;
    }
    public static Vector4 Normalize(Vector4 vec)
    {
        return vec.Normalized();
    }

    public readonly Vector4 Clamped(Vector4 min, Vector4 max)
    {
        float x = X < min.X ? min.X : X > max.X ? max.X : X;
        float y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
        float z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
        float w = W < min.W ? min.W : W > max.W ? max.W : W;
        return new(x, y, z, w);
    }
    public void Clamp(Vector4 min, Vector4 max)
    {
        X = X < min.X ? min.X : X > max.X ? max.X : X;
        Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
        Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
        W = W < min.W ? min.W : W > max.W ? max.W : W;
    }

    public static Vector4 Clamp(Vector4 vec, Vector4 min, Vector4 max)
    {
        float x = vec.X < min.X ? min.X : vec.X > max.X ? max.X : vec.X;
        float y = vec.Y < min.Y ? min.Y : vec.Y > max.Y ? max.Y : vec.Y;
        float z = vec.Z < min.Z ? min.Z : vec.Z > max.Z ? max.Z : vec.Z;
        float w = vec.W < min.W ? min.W : vec.W > max.W ? max.W : vec.W;
        return new(x, y, z, w);
    }

    public static float Dot(Vector4 a, Vector4 b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
    }

    public static float Angle(Vector4 a, Vector4 b)
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
                3 => W,
                _ => 0
            };
        }
        set
        {
            switch (index)
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
                case 3:
                    W = value;
                    break;
            }
        }
    }

    public static implicit operator System.Numerics.Vector4(Vector4 v)
        => new(v.X, v.Y, v.Z, v.W);
    public static implicit operator OpenTK.Mathematics.Vector4(Vector4 v)
        => new(v.X, v.Y, v.Z, v.W);
    public static explicit operator OpenTK.Mathematics.Vector4i(Vector4 v)
        => new((int)v.X, (int)v.Y, (int)v.Z, (int)v.W);

    public static implicit operator Vector4(System.Numerics.Vector4 v)
        => new(v.X, v.Y, v.Z, v.W);
    public static implicit operator Vector4(OpenTK.Mathematics.Vector4 v)
        => new(v.X, v.Y, v.Z, v.W);
    public static implicit operator Vector4(OpenTK.Mathematics.Vector4i v)
        => new(v.X, v.Y, v.Z, v.W);

    public static Vector4 operator +(Vector4 a)
        => a;
    public static Vector4 operator -(Vector4 a)
        => new(-a.X, -a.Y, -a.Z, -a.W);

    public static Vector4 operator +(Vector4 a, Vector4 b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    public static Vector4 operator -(Vector4 a, Vector4 b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    public static Vector4 operator *(Vector4 a, Vector4 b)
        => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
    public static Vector4 operator /(Vector4 a, Vector4 b)
        => new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);

    public static Vector4 operator +(Vector4 a, float b)
        => new(a.X + b, a.Y + b, a.Z + b, a.W + b);
    public static Vector4 operator -(Vector4 a, float b)
        => new(a.X - b, a.Y - b, a.Z - b, a.W - b);
    public static Vector4 operator *(Vector4 a, float b)
        => new(a.X * b, a.Y * b, a.Z * b, a.W * b);
    public static Vector4 operator /(Vector4 a, float b)
        => new(a.X / b, a.Y / b, a.Z / b, a.W / b);

    public static Vector4 operator +(float a, Vector4 b)
        => new(a + b.X, a + b.Y, a + b.Z, a + b.W);
    public static Vector4 operator -(float a, Vector4 b)
        => new(a - b.X, a - b.Y, a - b.Z, a - b.W);
    public static Vector4 operator *(float a, Vector4 b)
        => new(a * b.X, a * b.Y, a * b.Z, a * b.W);
    public static Vector4 operator /(float a, Vector4 b)
        => new(a / b.X, a / b.Y, a / b.Z, a / b.W);

    public static Vector4 operator ++(Vector4 a)
        => new(a.X++, a.Y++, a.Z++, a.W++);
    public static Vector4 operator --(Vector4 a)
        => new(a.X--, a.Y--, a.Z--, a.W--);

    public readonly override string ToString()
        => $"<{X}, {Y}, {Z}, {W}>";

    public void operator +=(Vector4 a)
    {
        X += a.X;
        Y += a.Y;
        Z += a.Z;
        W += a.W;
    }
    public void operator -=(Vector4 a)
    {
        X -= a.X;
        Y -= a.Y;
        Z -= a.Z;
        W -= a.W;
    }
    public void operator *=(Vector4 a)
    {
        X *= a.X;
        Y *= a.Y;
        Z *= a.Z;
        W *= a.W;
    }
    public void operator /=(Vector4 a)
    {
        if (a.X == 0 || a.Y == 0 || a.Z == 0 || a.W == 0)
            throw new DivideByZeroException();
        X /= a.X;
        Y /= a.Y;
        Z /= a.Z;
        W /= a.W;
    }

    public void operator +=(float a)
    {
        X += a;
        Y += a;
        Z += a;
        W += a;
    }
    public void operator -=(float a)
    {
        X -= a;
        Y -= a;
        Z -= a;
        W -= a;
    }
    public void operator *=(float a)
    {
        X *= a;
        Y *= a;
        Z *= a;
        W *= a;
    }
    public void operator /=(float a)
    {
        if (a == 0)
            throw new DivideByZeroException();
        X /= a;
        Y /= a;
        Z /= a;
        W /= a;
    }
}