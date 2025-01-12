using System;
using System.IO;
using Silk.NET.OpenGL;

namespace OpenMC
{
    public class Shader : IDisposable
    {

        private uint _handle;
        private GL _gl;

        public Shader(GL gl, string vertPath, string fragPath)
        {
            _gl = gl;

            uint vertex = LoadShader(ShaderType.VertexShader, vertPath);
            uint frag = LoadShader(ShaderType.FragmentShader, fragPath);

            _handle = _gl.CreateProgram();

            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, frag);
            _gl.LinkProgram(_handle);

            _gl.GetProgram(_handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(_handle)}");
            }

            _gl.DetachShader(_handle, vertex);
            _gl.DetachShader(_handle, frag);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(frag);
        }

        public void Use()
        {
            _gl.UseProgram(_handle);
        }

        public void SetUniform(string name, int value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }

        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }

        public void Dispose()
        {
            _gl.DeleteProgram(_handle);
        }

        private uint LoadShader(ShaderType type, string path)
        {
            string src = File.ReadAllText(@"..\..\..\Shaders\" + path);
            uint handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, src);
            _gl.CompileShader(handle);

            string infoLog = _gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }
    }
}
