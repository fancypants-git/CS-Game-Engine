using Engine.Debugging;
using OpenTK.Graphics.OpenGL;
using Engine.Maths;
using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace Engine.Rendering;

public class Shader : IGpuResource
{
    protected Shader(ShaderSource[] shaders)
    {
        _sources = shaders.ToList();
        GpuResourceManager.Register(this);
    }

    private int _handle;
    public int Handle => _handle;

    private List<ShaderSource> _sources;

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public int GetUniformLocation(string name)
    {
        return GL.GetUniformLocation(_handle, name);
    }

    public void Uniform1f(string name, float value) => GL.Uniform1f(GetUniformLocation(name), value);
    public void Uniform2f(string name, float a, float b) => GL.Uniform2f(GetUniformLocation(name), a, b);
    public void Uniform3f(string name, float a, float b, float c) => GL.Uniform3f(GetUniformLocation(name), a, b, c);
    public void Uniform2fv(string name, Vector2 value) => GL.Uniform2f(GetUniformLocation(name), value.X, value.Y);
    public void Uniform3fv(string name, Vector3 value) => GL.Uniform3f(GetUniformLocation(name), value.X, value.Y, value.Z);
    public void Uniform1i(string name, int value) => GL.Uniform1i(GetUniformLocation(name), value);
    public void Uniform2i(string name, int a, int b) => GL.Uniform2i(GetUniformLocation(name), a, b);
    public void Uniform3i(string name, int a, int b, int c) => GL.Uniform3i(GetUniformLocation(name), a, b, c);
    public void UniformMat4(string name, bool transpose, Matrix4 value) => GL.UniformMatrix4f(GetUniformLocation(name), 1, transpose, ref value);

    
    public static bool ReportShaderCompileStatus(int shader)
    {
        GL.GetShaderi(shader, ShaderParameterName.CompileStatus, out var success);
        if (success == 0)
        {
            GL.GetShaderInfoLog(shader, out var log);
            Debug.LogErr($"Failed to Compile Shader: {shader}");
            Debug.LogErr(log);
        }

        return success == 1;
    }

    public static bool ReportProgramLinkStatus(int program)
    {
        GL.GetProgrami(program, ProgramProperty.LinkStatus, out var success);
        if (success == 0)
        {
            GL.GetProgramInfoLog(program, out var log);
            Debug.LogErr($"Failed to Link Program: {program}");
            Debug.LogErr(log);
        }

        return success == 1;
    }




    public static Shader Create(string vertPath, string fragPath)
    {
        ShaderSource[] sources = [
            ShaderSource.FromFile(vertPath, ShaderType.VertexShader),
            ShaderSource.FromFile(fragPath, ShaderType.FragmentShader)
        ];

        return new Shader(sources);
    }

    public bool IsInitialized { get; private set; }
    public bool IsDisposed { get; private set; }
    private RenderContext _context;
    public RenderContext Context => _context;

    public bool Initialize(RenderContext context)
    {
        if (IsInitialized) return true;

        _context = context;

        // create a shader program
        // 
        // for each shader source
        // create a new shader and assign the source to it
        // compile the shader and check compile status
        // attach the shader to the shader program
        // 
        // link the shader program
        // check the link status
        // 
        // detatch and delete all compiled shaders

        int tempHandle = GL.CreateProgram();
        List<int> compiledShaders = [];

        foreach (ShaderSource source in _sources)
        {
            int shader = GL.CreateShader(source.Type);
            GL.ShaderSource(shader, source.Source);
            GL.CompileShader(shader);
            if (!ReportShaderCompileStatus(shader))
            {
                GL.DeleteShader(shader);
                continue;
            }

            compiledShaders.Add(shader);
            GL.AttachShader(tempHandle, shader);
        }

        GL.LinkProgram(tempHandle);

        foreach (int shader in compiledShaders)
        {
            GL.DetachShader(tempHandle, shader);
            GL.DeleteShader(shader);
        }

        if (!ReportProgramLinkStatus(tempHandle))
        {
            GL.DeleteProgram(tempHandle);
            return false;
        }

        context.Register(this);
        _handle = tempHandle;
        IsInitialized = true;
        return true;
    }

    public void Dispose()
    {
        if (IsDisposed || !IsInitialized || !_context.IsAlive) return;

        GL.DeleteProgram(_handle);
        GpuResourceManager.UnRegister(this);
    }
    ~Shader()
    {
        if (!IsDisposed && IsInitialized)
            Debug.LogMemLeak(GetType().Name);
    }
}
