namespace Engine.Rendering;

/// <summary>
/// Contains the offset and amount of indices to form a range of indices
/// </summary>
public struct Submesh
{
    public int IndexStart;
    public int IndexCount;

    public Submesh(int indexStart, int indexCount)
    {
        IndexStart = indexStart;
        IndexCount = indexCount;
    }
}