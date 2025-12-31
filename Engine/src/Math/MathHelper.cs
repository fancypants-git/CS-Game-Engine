using TK = OpenTK.Mathematics;
using Numerics = System.Numerics;

namespace Engine.Maths;

public static class MathHelper
{
    public const float DegToRad = MathF.PI / 180f;
    public const float RadToDeg = 180f / MathF.PI;



    public static Numerics.Matrix4x4 ToSystem(this TK.Matrix4 value)
    {
        return new(
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44
        );
    }

    public static TK.Matrix4 ToOpenTK(this Numerics.Matrix4x4 value)
    {
        return new TK.Matrix4(
            value.M11, value.M12, value.M13, value.M14,
            value.M21, value.M22, value.M23, value.M24,
            value.M31, value.M32, value.M33, value.M34,
            value.M41, value.M42, value.M43, value.M44
        );
    }

    public static Numerics.Quaternion ToSystem(this TK.Quaternion value)
    {
        return new(value.X, value.Y, value.Z, value.W);
    }

    public static TK.Quaternion ToOpenTK(this Numerics.Quaternion value)
    {
        return new(value.X, value.Y, value.Z, value.W);
    }

    public static Vector3 ToAxisAngle(this Numerics.Quaternion value, out float angle)
    {
        /** Quaternion to Axis angle
         * 
         * The conversion from Quaternion to Axis angles can be done
         * using the following formula for a normalized quaternion:
         * 
         * Angle = 2 * acos(qw)
         * X = qx / sqrt(1 - qw*qw)
         * Y = qy / sqrt(1 - qw*qw)
         * Z = qz / sqrt(1 - qw*qw)
         * 
         * Sources
         * https://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToAngle/index.htm
         * https://en.wikipedia.org/wiki/Quaternions_and_spatial_rotation
         */

        if (value.LengthSquared() > 0f)
            value = Numerics.Quaternion.Normalize(value);

        angle = 2 * MathF.Acos(value.W);
        float scalar = MathF.Sqrt(1 - value.W * value.W);

        if (scalar < 1e-6f) // test to avoid divide by 0
        {
            return new Vector3(1, 0, 0).Normalized();
        }

        return new Vector3(value.X / scalar, value.Y / scalar, value.Z / scalar).Normalized();
    }

    public static Vector3 ToEulerAngles(this Numerics.Quaternion value)
    {
        /** Quaternion to Euler conversion
         * 
         * A conversion from Quaternion to Euler angles can be done
         * using the following formula:
         * 
         * Heading = atan2(2(qw*qx + qy*qz), 1 - 2(qx*qx + qy*qy))
         * Pitch = -PI/2 + 2atan2(sqrt(1 + 2(qw*qy - qx*qz), sqrt(1 - 2(qw*qy - qx*qz))))
         * Bank = atan2(2(qw*qz + qx*qy), 1 - 2(qy*qy + qz*qz))
         * following the Tait-Bryan angles (see below).
         *
         * But you must be aware of the singularities at the north and south pole,
         * a north or south pole facing quaternion can be determined by checking if
         * qx*qy + qz*qw is outside of the mathmatical range < -0.5, 0.5 >,
         * this range is valid when the quaternion is normalised, if not, the range must be multiplied by the quaternions unit.
         * These can be handled using the following formulas:
         * - NORTH POLE
         *      Heading = 2 * atan2(qx, qw)
         *      Pitch = PI / 2
         *      Bank = 0
         * - SOUTH POLE
         *      Heading = -2 * atan2(qx, qw)
         *      Pitch = -PI / 2
         *      Bank = 0
         * 
         *
         * Tait-Bryan angles
         * 
         * These angles are similar to Eurler angles, as they describe the axes of rotation:
         * Heading - psi:                   rotation about the Z-axis
         * Pitch (aka "attitude") - theta:  rotation about the Y-axis
         * Bank - phi:                      rotation about the X-axis
         * 
         * However these axes have a different meaning than the Euler X, Y and Z axes,
         * as the Heading (psi/Z-axis) points downwards, the Pitch (theta/Y-axis) points to the right
         * and the Bank (phi/X-axis) points forward.
         * 
         * This means that the conversion from Tait-Bryan to Euler angles is:
         * EulerX = Pitch
         * EulerY = -Heading
         * EulerZ = Bank
         * 
         * 
         * Sources
         * https://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToEuler/
         * https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
         */

        const float singularityThreshold = 0.4999995f;

        float sqw = value.W * value.W;
        float sqx = value.X * value.X;
        float sqy = value.Y * value.Y;
        float sqz = value.Z * value.Z;

        // this is the unit as refered to before,
        // it consists of the sum of the squared values of each of the four components in the quaternion (x, y, z and w)
        float unit = sqx + sqy + sqz + sqw;
        float singularityTest = value.X * value.Z + value.Y * value.W;

        float heading;
        float pitch;
        float bank;

        if (singularityTest > singularityThreshold * unit) // singularity at north pole
        {
            heading = 2 * MathF.Atan2(value.X, value.W);
            pitch = MathF.PI / 2;
            bank = 0;
        }
        else if (singularityTest < -singularityThreshold * unit) // singularity at south pole
        {
            heading = -2 * MathF.Atan2(value.X, value.W);
            pitch = -MathF.PI / 2;
            bank = 0;
        }
        else // no singularities
        {
            heading = MathF.Atan2(2 * value.Y * value.W - 2 * value.X * value.Z, 1 - 2 * sqy - 2 * sqz);
            pitch = MathF.Asin(2 * value.X * value.Y + 2 * value.Z * value.W);
            bank = MathF.Atan2(2 * value.X * value.W - 2 * value.Y * value.Z, 1 - 2 * sqx - 2 * sqz);
        }

        return new Vector3(pitch, -heading, bank);
    }
}