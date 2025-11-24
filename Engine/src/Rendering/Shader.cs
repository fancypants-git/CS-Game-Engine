using Engine.Helpers;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Engine.Rendering;

public class Shader : IDisposable
{
    public readonly int Handle;

    private bool _isDisposed = false;

    public Shader(string vertPath, string fragPath)
    {
        // read the sources as text
        var vertSource = File.ReadAllText(vertPath);
        var fragSource = File.ReadAllText(fragPath);

        // create the shaders from the shader sources
        var vertShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertShader, vertSource);
        var fragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragShader, fragSource);

        // compile the shaders and report their status
        GL.CompileShader(vertShader);
        GL.CompileShader(fragShader);

        if (!ReportShaderCompileStatus(vertShader) ||
            !ReportShaderCompileStatus(fragShader)) // if any of the two reports fails
        {
            Dispose(true);
        }

        // create and compile the program
        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertShader);
        GL.AttachShader(Handle, fragShader);

        GL.LinkProgram(Handle);

        GL.DetachShader(Handle, vertShader);
        GL.DetachShader(Handle, fragShader);
        GL.DeleteShader(vertShader);
        GL.DeleteShader(fragShader);

        if (!ReportProgramLinkStatus(Handle))
        {
            Dispose(true);
            Handle = 0;
        }
    }


    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public int GetUniformLocation(string name)
    {
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

    
    public bool ReportShaderCompileStatus(int shader)
    {
        GL.GetShaderi(shader, ShaderParameterName.CompileStatus, out var success);
        if (success == 0)
        {
            GL.GetShaderInfoLog(shader, out var log);
            Debug.LogError("Failed to Compile Shader:", shader, '\n', log);
        }

        return success == 1;
    }

    public bool ReportProgramLinkStatus(int program)
    {
        GL.GetProgrami(program, ProgramProperty.LinkStatus, out var success);
        if (success == 0)
        {
            GL.GetProgramInfoLog(program, out var log);
            Debug.LogError("Failed to Link Program:", program, '\n', log);
        }

        return success == 1;
    }


    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing) GL.DeleteProgram(Handle);

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

        Debug.LogMemLeak("Shader");
        Dispose(false);
    }
}