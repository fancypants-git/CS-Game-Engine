using SysVec3 = System.Numerics.Vector3;
using JoltPhysicsSharp;
using Engine.Maths;

namespace Engine.Debugging;

public class JoltDebugRenderer : DebugRenderer
{
    protected override void DrawLine(SysVec3 from, SysVec3 to, JoltColor color)
    {
        Debug.DrawLine(from, to, color);
    }

    protected override void DrawText3D(SysVec3 position, string? text, JoltColor color, float height = 0.5F)
    {
    }

    protected override void DrawTriangle(SysVec3 v1, SysVec3 v2, SysVec3 v3, JoltColor color, CastShadow castShadow = CastShadow.Off)
    {
        Debug.DrawTriangle(v1, v2, v3, color);
    }
}
