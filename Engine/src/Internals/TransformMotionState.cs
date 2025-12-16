using BulletSharp;
using BulletSharp.Math;
using Engine.Components;

namespace Engine.Internals;

public class TransformMotionState : MotionState
{
    private Transform transform;
    
    public TransformMotionState(Transform t)
    {
        this.transform = t;
    }

    public override void GetWorldTransform(out Matrix worldTrans)
    {
        worldTrans = ToBulletMatrix(transform.TranslationMatrix * transform.RotationMatrix);
    }

    public override void SetWorldTransform(ref Matrix worldTrans)
    {
        OpenTK.Mathematics.Vector3 position;
        GetPositionAndRotation(worldTrans, out position, out var rot);
        transform.GlobalPosition = position;
        transform.GlobalRotation = QuaternionToEuler(rot);
    }
    

    public static BulletSharp.Math.Matrix ToBulletMatrix(OpenTK.Mathematics.Matrix4 m)
    {
        return new BulletSharp.Math.Matrix(
            m.M11, m.M12, m.M13, m.M14,
            m.M21, m.M22, m.M23, m.M24,
            m.M31, m.M32, m.M33, m.M34,
            m.M41, m.M42, m.M43, m.M44
        );
    }
    
    public static void GetPositionAndRotation(Matrix worldTrans, out OpenTK.Mathematics.Vector3 position, out Quaternion rotation)
    {
        worldTrans.Decompose(out Vector3 scale, out Quaternion rot, out Vector3 trans);
        position = new OpenTK.Mathematics.Vector3(trans.X, trans.Y, trans.Z);
        rotation = new Quaternion(rot.X, rot.Y, rot.Z, rot.W);
    }
    public static OpenTK.Mathematics.Vector3 QuaternionToEuler(Quaternion q)
    {
        // OpenTK uses radians
        float ysqr = q.Y * q.Y;

        // Roll (X-axis rotation)
        float t0 = +2.0f * (q.W * q.X + q.Y * q.Z);
        float t1 = +1.0f - 2.0f * (q.X * q.X + ysqr);
        float roll = MathF.Atan2(t0, t1);

        // Pitch (Y-axis rotation)
        float t2 = +2.0f * (q.W * q.Y - q.Z * q.X);
        t2 = Math.Clamp(t2, -1f, 1f);
        float pitch = MathF.Asin(t2);

        // Yaw (Z-axis rotation)
        float t3 = +2.0f * (q.W * q.Z + q.X * q.Y);
        float t4 = +1.0f - 2.0f * (ysqr + q.Z * q.Z);
        float yaw = MathF.Atan2(t3, t4);

        return new OpenTK.Mathematics.Vector3(roll, pitch, yaw);
    }

}
