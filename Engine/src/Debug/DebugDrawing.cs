using Engine.Helpers;
using Engine.Physics;
using Engine.Rendering;
using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using Engine.Components;
using System.Drawing;

namespace Engine.Debugging;

public static partial class Debug
{
    /*
     * REQUIREMENTS TO DRAW OPENGL CONTEXT
     * - VAO
     * - Shader
     * - Vertex Data (VBO)
     * - Index Data (EBO)
     * 
     * STEPS TO DRAW OPENGL CONTEXT
     * 1. Initialize Shader
     * 2. Initialize VAO
     * 3. Initialze VBO / EBO
     * 4. Initialize Pointers
     * 5. Set Vertex Data
     * 6. Set Uniform Data
     * 7. Call OpenGL Draw Method
     */
    
    
    public static bool EnableDebugDrawing = true; // Set to false only if Debug.Draw*() will never be used (e.g. in release builds)
    
    private static List<DebugVertex> _debugDrawLineVertices = [];
    private static List<uint> _debugDrawLineIndices = [];
    
    private readonly static VertexArrayObject _debugDrawLineVAO = new();
    private readonly static VertexBufferObject _debugDrawLineVBO = new();
    private readonly static ElementBufferObject _debugDrawLineEBO = new();
    private readonly static Shader _debugDrawLineShader = Resources.GetShader("debug.line");
    
    static Debug()
    {
        int stride = 6 * sizeof(float);
        _debugDrawLineVAO.Use();
        _debugDrawLineVBO.Use();
        _debugDrawLineEBO.Use();
        _debugDrawLineVAO.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
        _debugDrawLineVAO.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
        GL.LineWidth(1.0f);
    }
    
    // Rendering Method!
    // This renders all requested Debug Draws
    public static void Render(Camera camera)
    {
        _debugDrawLineVAO.Use();
        _debugDrawLineVBO.Upload(_debugDrawLineVertices.ToArray(), BufferUsage.DynamicDraw);
        _debugDrawLineEBO.Upload(_debugDrawLineIndices.ToArray(), BufferUsage.DynamicDraw);
        
        _debugDrawLineShader.Use();
        _debugDrawLineShader.UniformMat4("view", false, camera.View);
        _debugDrawLineShader.UniformMat4("projection", false, camera.Projection);
        
        GL.DrawElements(PrimitiveType.Lines, _debugDrawLineIndices.Count, DrawElementsType.UnsignedInt, 0);
        
        _debugDrawLineIndices.Clear();
        _debugDrawLineVertices.Clear();
    }
    
    public static void DrawLine(Vector3 a, Vector3 b, Color color)
    {
        _debugDrawLineIndices.Add((uint)_debugDrawLineVertices.Count);
        _debugDrawLineIndices.Add((uint)_debugDrawLineVertices.Count + 1);
        _debugDrawLineVertices.Add(new DebugVertex { Position = a, Color = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f)});
        _debugDrawLineVertices.Add(new DebugVertex { Position = b, Color = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f)});
    }
    
    public static void DrawBounds(Bounds bounds, Color color)
    {
        Vector3 min = bounds.Min;
        Vector3 max = bounds.Max;

        Vector3 c = new Vector3(
            color.R / 255f,
            color.G / 255f,
            color.B / 255f
        );

        // Remember starting index (VERY important)
        uint baseIndex = (uint)_debugDrawLineVertices.Count;

        // 8 corners
        _debugDrawLineVertices.Add(new DebugVertex { Position = new Vector3(min.X, min.Y, min.Z), Color = c }); // 0
        _debugDrawLineVertices.Add(new DebugVertex { Position = new Vector3(max.X, min.Y, min.Z), Color = c }); // 1
        _debugDrawLineVertices.Add(new DebugVertex { Position = new Vector3(max.X, max.Y, min.Z), Color = c }); // 2
        _debugDrawLineVertices.Add(new DebugVertex { Position = new Vector3(min.X, max.Y, min.Z), Color = c }); // 3
        _debugDrawLineVertices.Add(new DebugVertex { Position = new Vector3(min.X, min.Y, max.Z), Color = c }); // 4
        _debugDrawLineVertices.Add(new DebugVertex { Position = new Vector3(max.X, min.Y, max.Z), Color = c }); // 5
        _debugDrawLineVertices.Add(new DebugVertex { Position = new Vector3(max.X, max.Y, max.Z), Color = c }); // 6
        _debugDrawLineVertices.Add(new DebugVertex { Position = new Vector3(min.X, max.Y, max.Z), Color = c }); // 7

        // Bottom face
        _debugDrawLineIndices.Add(baseIndex + 0);
        _debugDrawLineIndices.Add(baseIndex + 1);

        _debugDrawLineIndices.Add(baseIndex + 1);
        _debugDrawLineIndices.Add(baseIndex + 2);

        _debugDrawLineIndices.Add(baseIndex + 2);
        _debugDrawLineIndices.Add(baseIndex + 3);

        _debugDrawLineIndices.Add(baseIndex + 3);
        _debugDrawLineIndices.Add(baseIndex + 0);

        // Top face
        _debugDrawLineIndices.Add(baseIndex + 4);
        _debugDrawLineIndices.Add(baseIndex + 5);

        _debugDrawLineIndices.Add(baseIndex + 5);
        _debugDrawLineIndices.Add(baseIndex + 6);

        _debugDrawLineIndices.Add(baseIndex + 6);
        _debugDrawLineIndices.Add(baseIndex + 7);

        _debugDrawLineIndices.Add(baseIndex + 7);
        _debugDrawLineIndices.Add(baseIndex + 4);

        // Vertical edges
        _debugDrawLineIndices.Add(baseIndex + 0);
        _debugDrawLineIndices.Add(baseIndex + 4);

        _debugDrawLineIndices.Add(baseIndex + 1);
        _debugDrawLineIndices.Add(baseIndex + 5);

        _debugDrawLineIndices.Add(baseIndex + 2);
        _debugDrawLineIndices.Add(baseIndex + 6);

        _debugDrawLineIndices.Add(baseIndex + 3);
        _debugDrawLineIndices.Add(baseIndex + 7);
    }
}
