using OpenTK.Graphics.OpenGL;

namespace Engine;

/// <summary>
/// Submesh contains index data of a part of a Mesh<br/>
/// this includes:<br/>
/// ElementBufferObject, indices
/// </summary>
public struct Submesh
{
    public int IndexStart { get; set; }
    public int IndexCount { get; set; }

    public Submesh(int indexStart, int indexCount)
    {
        IndexStart = indexStart;
        IndexCount = indexCount;
    }
}