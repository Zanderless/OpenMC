using OpenMC.World;
using Silk.NET.OpenGL;
using System.Numerics;

namespace OpenMC.Rendering
{
    public class Renderer : IDisposable
    {
        private GL _gl;

        private BufferObject<float> _vbo;
        private BufferObject<uint> _ebo;
        private VertexArrayObject<float, uint> _vao;

        private float[] _meshData;
        private uint[] _indices;

        private Shader _shader;
        private Texture _texture;

        private Camera _activeCamera;

        public void SetMeshData(float[] meshData, uint[] indices)
        {
            _meshData = meshData;
            _indices = indices;
        }

        public void SetShader(string vertPath, string fragPath)
        {
            _shader = new Shader(_gl, vertPath, fragPath);
        }

        public void CreateMesh()
        {
            _ebo = new BufferObject<uint>(_gl, _indices, BufferTargetARB.ElementArrayBuffer); //Vertex indices
            _vbo = new BufferObject<float>(_gl, _meshData, BufferTargetARB.ArrayBuffer); //Mesh Data
            _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);

            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 9, 0);
            _vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 9, 3);
            _vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 9, 6);

            _texture = new Texture(_gl, 16, 16);

            _gl.Enable(EnableCap.CullFace);
            _gl.CullFace(GLEnum.Back);
        }

        public void RenderMesh(uint vertexCount)
        {
            _vao.Bind();

            _shader.Use();
            _texture.Bind(TextureTarget.Texture2DArray);

            _shader.SetUniform("uModel", Matrix4x4.Identity);
            _shader.SetUniform("uView", _activeCamera.GetViewMatrix());
            _shader.SetUniform("uProjection", _activeCamera.GetProjectionMatrix());
            _shader.SetUniform("uTexture", 0);
            _shader.SetUniform("uLightColor", Vector3.One);
            _shader.SetUniform("uLightPos", Vector3.Zero);
            _shader.SetUniform("uLayerCount", 3);

            _gl.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
        }

        public Renderer(GL gl, Camera camera)
        {
            _gl = gl;
            _activeCamera = camera;
        }

        public void Dispose()
        {
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            _shader.Dispose();
        }
    }
}
