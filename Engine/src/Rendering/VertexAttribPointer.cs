using OpenTK.Graphics.OpenGL;

namespace Engine.Rendering;

public struct VertexAttribPointer
{
    public VertexAttribPointer(uint location, int size, VertexAttribPointerType type, bool normalized, int stride, int offset)
    {
        Location = location;
        Size = size;
        Type = type;
        Normalized = normalized;
        Stride = stride;
        Offset = offset;
    }

    public uint Location;
    public int Size;
    public VertexAttribPointerType Type;
    public bool Normalized;
    public int Stride;
    public int Offset;
}