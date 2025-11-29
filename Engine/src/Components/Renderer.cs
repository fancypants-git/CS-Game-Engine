using System.Drawing;
using Engine.Attributes;
using Engine.Helpers;
using Engine.Interfaces;
using Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Components;

[ComponentMeta("Renderer")]
public class Renderer : Component, IDrawable
{
	// TODO Add a MaterialPropertyBlock system (like Unity) so multiple renderers can share a material but override a few properties (e.g., color).
	// TODO Add a RendererType distinction later (MeshRenderer, SkinnedMeshRenderer, SpriteRenderer, etc.).
	// TODO Add batching logic later based on shared Mesh and Material.
	// TODO Add automatic material/indices combination to combine submeshes with the same Material
	
	private static readonly Vertex[] cubeVertices =
	{
	    // ─────────────────────────────
	    // FRONT
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(0f, 0f, 1f) },
	    new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(0f, 0f, 1f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(0f, 0f, 1f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(0f, 0f, 1f) },

	    // ─────────────────────────────
	    // BACK
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(0f, 0f, -1f) },
	    new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(0f, 0f, -1f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(0f, 0f, -1f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(0f, 0f, -1f) },

	    // ─────────────────────────────
	    // LEFT
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(-1f, 0f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(-1f, 0f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(-1f, 0f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(-1f, 0f, 0f) },

	    // ─────────────────────────────
	    // RIGHT
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(1f, 0f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(1f, 0f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(1f, 0f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(1f, 0f, 0f) },

	    // ─────────────────────────────
	    // BOTTOM
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3(-0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(0f, -1f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f, -0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(0f, -1f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(0f, -1f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f, -0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(0f, -1f, 0f) },

	    // ─────────────────────────────
	    // TOP
	    // ─────────────────────────────
	    new Vertex { Position = new Vector3(-0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(0.0f, 0.0f), Normal = new Vector3(0f, 1f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f,  0.5f), TextureCoordinate = new Vector2(1.0f, 0.0f), Normal = new Vector3(0f, 1f, 0f) },
	    new Vertex { Position = new Vector3( 0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(1.0f, 1.0f), Normal = new Vector3(0f, 1f, 0f) },
	    new Vertex { Position = new Vector3(-0.5f,  0.5f, -0.5f), TextureCoordinate = new Vector2(0.0f, 1.0f), Normal = new Vector3(0f, 1f, 0f) }
	};



    private static uint[] cubeIndices = [
		0, 1, 2, 2, 3, 0, // Front
		4, 5, 6, 6, 7, 4, // Back
		8, 9, 10, 10, 11, 8, // Left
		12, 13, 14, 14, 15, 12, // Right
		16, 17, 18, 18, 19, 16, // Bottom
		20, 21, 22, 22, 23, 20 // Top
    ];
    
    
    public Material[] Materials { get; set; }
    public Mesh Mesh { get; set; }
    

    public Renderer(Entity entity) : base(entity) // default constructor
    {
	    Materials = [
		    new Material
		    {
			    Shader = Resources.GetShader("lit"),
			    Texture = null,
			    Color = Color.White
		    }
	    ];
	    
	    Mesh = new Mesh(cubeVertices, cubeIndices);
	    var stride = 8 * sizeof(float);
	    Mesh.VertexArrayObject.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
	    Mesh.VertexArrayObject.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
	    Mesh.VertexArrayObject.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, stride, 5 * sizeof(float));
    }

    public Renderer(Entity entity, Mesh mesh, Material[] materials) : base(entity)
    {
	    Materials = materials;
	    for (int i = 0; i < materials.Length; i++)
	    {
		    materials[i].Shader = Resources.GetShader("lit");
	    }
	    Mesh = mesh;
    }

    public Renderer(Entity entity, Mesh mesh, Material[] materials, Shader shader) : base(entity)
    {
	    Materials = materials;
	    for (int i = 0; i < materials.Length; i++)
	    {
		    materials[i].Shader = shader;
	    }
	    Mesh = mesh;
    }
	
    public void Draw(Camera camera)
    {
	    if (Mesh.Submeshes.Length == 0)
	    {
		    var mat = Materials[0];
		    mat.Use(camera, Transform.ModelMatrix);
		    GL.DrawArrays(PrimitiveType.Triangles, 0, Mesh.Submeshes.Length);
		    return;
	    }
	    
	    for (var i = 0; i < Mesh.Submeshes.Length; i++)
	    {
		    var mat = Materials[i];
		    mat.Use(camera, Transform.ModelMatrix);
		    Mesh.DrawSubmesh(i);
	    }
    }


    protected override void Dispose(bool disposing)
    {
	    if (IsDisposed) return;
	    
	    if (disposing)
	    {
		    for (int i = 0; i < Materials.Length; i++)
			    Materials[i].Dispose();

		    Mesh.Dispose();
	    }
	    
	    IsDisposed = true;
    }

    ~Renderer()
    {
	    if (IsDisposed) return;
	    
	    Debug.LogMemLeak("Renderer");
	    Dispose(false);
    }
}