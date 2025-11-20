using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;

namespace Engine;

/// <summary>
/// Mesh contains the vertex data of a mesh<br/>
/// this includes:<br/>
/// (basics) VertexArrayObject, VertexBufferObject, Vertices<br/>
/// (optional) Submesh[]<br/>
/// The Submesh[] contains the index data of the submeshes and each Submesh is usually paired with a Material in any IDrawable
/// </summary>
public struct Mesh : IDisposable
{
    private Vertex[] _vertices;
    public Vertex[] Vertices
    {
        get
        {
            return _vertices;
        }
        set
        {
            _vertices = value;
            VertexBufferObject.Upload(_vertices, BufferUsage.StaticDraw);
        }
    }
    private uint[] _indices;
    public uint[] Indices
    {
        get
        {
            return _indices;
        }
        set
        {
            _indices = value;
            ElementBufferObject.Upload(_indices, BufferUsage.StaticDraw);
        }
    }
    
    
    public VertexArrayObject VertexArrayObject { get; private set; }
    public VertexBufferObject VertexBufferObject { get; private set; }
    public ElementBufferObject ElementBufferObject { get; private set; }
    
    public Submesh[] Submeshes { get; }
    
    private bool _isDisposed = false;

    public Mesh(Vertex[] vertices, uint[]? indices)
    {
        _vertices = vertices;
        _indices = indices;

        if (indices != null)
            Submeshes = [new Submesh(0, indices.Length)];
        else Submeshes = [];
        
        Initialize();
    }
    public Mesh(Vertex[] vertices, uint[] indices, Submesh[] submeshes)
    {
        _vertices = vertices;
        _indices = indices;
        
        Submeshes = submeshes;

        Initialize();
    }

    private void Initialize()
    {
        VertexArrayObject = new VertexArrayObject();
        VertexArrayObject.Use();
        VertexBufferObject = new VertexBufferObject();
        VertexBufferObject.Upload(_vertices, BufferUsage.StaticDraw);

        if (_indices != null)
        {
            ElementBufferObject = new ElementBufferObject();
            ElementBufferObject.Upload(_indices, BufferUsage.StaticDraw);
        }

        const int stride = 8 * sizeof(float);
        VertexArrayObject.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        VertexArrayObject.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        VertexArrayObject.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
    }

    public void Use()
    {
        VertexArrayObject.Use();
    }

    public void DrawSubmesh(int index)
    {
        var sub = Submeshes[index];
        Use();
        GL.DrawElements(PrimitiveType.Triangles, sub.IndexCount, DrawElementsType.UnsignedInt, sub.IndexStart * sizeof(uint));
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;

        ElementBufferObject?.Dispose();
        VertexArrayObject.Dispose();
        VertexBufferObject.Dispose();
        
        _isDisposed = true;
    }
}