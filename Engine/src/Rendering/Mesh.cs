using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Engine.Debugging;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;

namespace Engine.Rendering;

/// <summary>
/// A mesh represents the shape of an object with vertices and (optionally) indices paired with submeshes.
/// Besides that a mesh includes the Vertex Object/Buffer and (combined with indices) Element Buffer
/// </summary>
public class Mesh : IGpuResource
{
    public Mesh(Vertex[] vertices, uint[]? indices)
    {
        _vertices = vertices;
        _indices = indices!;

        _submeshes = [];
        if (indices != null)
        {
            Submesh submesh = new(0, indices.Length);
            _submeshes = [submesh];
        }

        GpuResourceManager.Register(this);
    }
    public Mesh(Vertex[] vertices, uint[] indices, Submesh[] submeshes)
    {
        _vertices = vertices;
        _indices = indices;
        _submeshes = submeshes;

        GpuResourceManager.Register(this);
    }
    public Mesh() {}

    /// <summary>
    /// The vertices that make up this Mesh
    /// </summary>
    private Vertex[] _vertices;

    /// <summary>
    /// The indices (sometimes referred to as "faces") that form the triangles of this Mesh.
    /// These are optional but reccommended.
    /// </summary>
    private uint[] _indices;

    /// <summary>
    /// The different submeshes of this mesh, these refer to different slices in the indices array.
    /// These are optional, but one at minimum must be present if any indices are given.
    /// </summary>
    public Submesh[] Submeshes => _submeshes;
    private Submesh[] _submeshes;

    /// <inheritdoc cref="VertexArrayObject" />
    public VertexArrayObject VertexArray { get; private init; } = VertexArrayObject.Create();

    /// <inheritdoc cref="VertexBufferObject" />
    public VertexBufferObject VertexBuffer { get; private init; } = VertexBufferObject.Create();

    /// <inheritdoc cref="ElementBufferObject" />
    public ElementBufferObject ElementBuffer { get; private init; } = ElementBufferObject.Create();


    /// <summary>
    /// Set the vertices of this mesh
    /// </summary>
    /// <param name="vertices">The new vertices</param>
    public void SetVertices(Vertex[] vertices)
    {
        _vertices = vertices;
        VertexBuffer.Upload(_vertices, BufferUsage.StaticDraw);
    }

    /// <summary>
    /// Gets the vertices of this mesh
    /// </summary>
    /// <returns>The vertices of this mesh</returns>
    public Vertex[] GetVertices()
    {
        return _vertices;
    }

    /// <summary>
    /// Sets the indices (and paired submeshes) of this mesh
    /// </summary>
    /// <param name="indices">The new indices</param>
    /// <param name="submeshes">The new submeshes paired with the indices</param>
    public void SetIndices(uint[] indices, Submesh[] submeshes)
    {
        _indices = indices;
        _submeshes = submeshes;
        VertexArray.Use();
        ElementBuffer.Upload(_indices, BufferUsage.StaticDraw);
    }

    /// <summary>
    /// Gets the indices and paired submeshes of this mesh
    /// </summary>
    /// <param name="submeshes">The submeshes paired with the indices</param>
    /// <returns>The indices of this mesh</returns>
    public uint[] GetIndices(out Submesh[] submeshes)
    {
        submeshes = _submeshes;
        return _indices;
    }

    /// <summary>
    /// Draws this mesh with the submesh at INDEX
    /// </summary>
    /// <param name="index">The index of the submesh to draw</param>
    public void DrawSubmesh(int index)
    {
        Submesh submesh = _submeshes[index];
        VertexArray.Use();
        GL.DrawElements(PrimitiveType.Triangles, submesh.IndexCount, DrawElementsType.UnsignedInt, submesh.IndexStart);
    }



    /// <inheritdoc />
    public bool IsInitialized { get; private set; }
    /// <inheritdoc />
    public bool IsDisposed { get; private set; }
    
    /// <inheritdoc cref=IGpuResource.Context />
    private RenderContext _context;
    /// <inheritdoc />
    public RenderContext Context => _context;

    /// <inheritdoc />
    public bool Initialize(RenderContext context)
    {
        if (IsInitialized) return true;

        if (!VertexArray.IsInitialized || !VertexBuffer.IsInitialized
            || (_indices != null && !ElementBuffer.IsInitialized))
            return false;

        _context = context;

        VertexArray.Use();
        VertexBuffer.Upload(_vertices, BufferUsage.StaticDraw);

        int stride = Unsafe.SizeOf<Vertex>();
        VertexArray.AttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        VertexArray.AttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        VertexArray.AttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));

        if (_indices != null)
            ElementBuffer.Upload(_indices, BufferUsage.StaticDraw);

        context.Register(this);
        IsInitialized = true;
        return true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (IsDisposed || !IsInitialized || !_context.IsAlive) return;

        VertexArray.Dispose();
        VertexBuffer.Dispose();
        ElementBuffer.Dispose();

        GpuResourceManager.UnRegister(this);
        IsDisposed = true;
    }
    ~Mesh()
    {
        if (!IsDisposed && IsInitialized)
            Debug.LogMemLeak(GetType().Name);
    }
}