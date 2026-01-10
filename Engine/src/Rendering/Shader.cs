using Engine.Debugging;
using OpenTK.Graphics.OpenGL;
using Engine.Maths;
using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace Engine.Rendering;

public class Shader : IDisposable, IRequireRenderContext
{
    private int _handle;
    public int Handle => _handle;

    public bool IsInitialized { get; set; }

    private bool _isDisposed = false;

    private List<ShaderSource> _shaders = [];


    public Shader(string vertPath, string fragPath)
    {
        _shaders.Add(ShaderSource.FromFile(vertPath, ShaderType.VertexShader));
        _shaders.Add(ShaderSource.FromFile(fragPath, ShaderType.FragmentShader));
    }

    public bool Initialize()
    {
        if (!IRequireRenderContext.RenderContextExists()) return false;

        if (IsInitialized) return true;

        List<int> compiledShaders = [];

        _handle = GL.CreateProgram();

        foreach (ShaderSource shader in _shaders)
        {
            int compiledShader = GL.CreateShader(shader.Type);
            GL.ShaderSource(compiledShader, shader.Source);

            GL.CompileShader(compiledShader);

            if (!ReportShaderCompileStatus(compiledShader))
            {
                Dispose(true);
                return false;
            }

            GL.AttachShader(_handle, compiledShader);
            compiledShaders.Add(compiledShader);
        }

        GL.LinkProgram(_handle);

        if (!ReportProgramLinkStatus(_handle))
        {
            Dispose(true);
            return false;
        }

        foreach (int shader in compiledShaders)
        {
            GL.DetachShader(_handle, shader);
            GL.DeleteShader(shader);
        }

        IsInitialized = true;
        return true;
    }

    public void Use()
    {
        if (!Initialize()) return;
        GL.UseProgram(Handle);
    }

    public int GetUniformLocation(string name)
    {
        if (!Initialize()) return -1;
        return GL.GetUniformLocation(Handle, name);
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
            Debug.LogErr("Failed to Compile Shader: ", shader.ToString());
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
            Debug.LogErr("Failed to Link Program:", program.ToString());
            Debug.LogErr(log);
        }

        return success == 1;
    }


    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing && IsInitialized) GL.DeleteProgram(Handle);

        _isDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Shader()
    {
        if (_isDisposed) return;

        Debug.LogMemLeak(GetType().Name);
        Dispose(false);
    }
}
