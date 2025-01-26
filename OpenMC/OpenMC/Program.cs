﻿using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using System.Drawing;
using System.Numerics;
using OpenMC.Blocks;
using OpenMC.World;

namespace OpenMC
{
    class Program
    {

        private static GL _gl;
        private static IWindow _window;
        private static IKeyboard _keyboard;

        private static BufferObject<float> _vbo;
        private static BufferObject<uint> _ebo;
        private static VertexArrayObject<float, uint> _vao;

        private static Shader _blockShader;
        private static Shader _lampShader;
        private static Vector3 _lampPosition = new Vector3(1.0f, 18.0f, -1.0f);

        private static Texture _texture;

        private static Camera _camera;
        private static Vector2 _lastMousePosition;

        private static Block _block;
        private static Chunk _chunk;

        public static void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(1200, 900),
                Title = "OpenMC - Indev 0.0.1"
            };

            _window = Window.Create(options);

            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.FramebufferResize += OnFramebufferResize;
            _window.Closing += OnClose;

            _window.Run();

            _window.Dispose();
        }

        private static unsafe void OnLoad()
        {
            _gl = _window.CreateOpenGL();
            _gl.ClearColor(Color.CornflowerBlue);

            _block = new Block(BlockType.cobblestone, Vector3.Zero);
            _chunk = new Chunk(32, 16, 32);

            _ebo = new BufferObject<uint>(_gl, _chunk.GetIndices(), BufferTargetARB.ElementArrayBuffer);
            _vbo = new BufferObject<float>(_gl, _chunk.GetMeshData(), BufferTargetARB.ArrayBuffer);
            _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);

            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 9, 0);
            _vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 9, 3);
            _vao.VertexAttributePointer(2, 3, VertexAttribPointerType.Float, 9, 6);

            _blockShader = new Shader(_gl, "shader.vert", "block.frag");
            _lampShader = new Shader(_gl, "shader.vert", "light.frag");

            _texture = new Texture(_gl, 16, 16);

            _gl.Enable(EnableCap.CullFace);
            _gl.CullFace(GLEnum.Back);

            var size = _window.FramebufferSize;

            Vector3 spawnPos = new Vector3(_chunk.Size.X / 2, _chunk.Size.Y + 2, _chunk.Size.Z / 2);
            //Vector3 spawnPos = new Vector3(0, 0, 5);

            _camera = new Camera(spawnPos, Vector3.UnitZ * -1, Vector3.UnitY, (float)size.X / size.Y);

            IInputContext input = _window.CreateInput();
            _keyboard = input.Keyboards.FirstOrDefault();
            if (_keyboard != null)
            {
                _keyboard.KeyDown += KeyDown;
            }

            for (int i = 0; i < input.Mice.Count; i++)
            {
                input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
                input.Mice[i].MouseMove += OnMouseMove;
            }
        }

        private static void OnUpdate(double deltaTime)
        {
            var moveSpeed = 5.5f * (float)deltaTime;

            if (_keyboard.IsKeyPressed(Key.W))
                _camera.Position += moveSpeed * _camera.Forward;
            else if(_keyboard.IsKeyPressed(Key.S))
                _camera.Position -= moveSpeed * _camera.Forward;

            if (_keyboard.IsKeyPressed(Key.D))
                _camera.Position += moveSpeed * _camera.Right;
            else if (_keyboard.IsKeyPressed(Key.A))
                _camera.Position -= moveSpeed * _camera.Right;
        }

        private static unsafe void OnRender(double deltaTime)
        {
            _gl.Enable(EnableCap.DepthTest);
            _gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            _vao.Bind();

            RenderMesh();
            //RenderLampCube();
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            _gl.Viewport(newSize);
            _camera.AspectRatio = (float)newSize.X / newSize.Y;
        }

        private static unsafe void RenderMesh()
        {
            _blockShader.Use();
            _texture.Bind(TextureTarget.Texture2DArray);

            _blockShader.SetUniform("uModel", Matrix4x4.Identity);
            _blockShader.SetUniform("uView", _camera.GetViewMatrix());
            _blockShader.SetUniform("uProjection", _camera.GetProjectionMatrix());
            _blockShader.SetUniform("uTexture", 0);
            _blockShader.SetUniform("uLightColor", Vector3.One);
            _blockShader.SetUniform("uLightPos", _lampPosition);
            _blockShader.SetUniform("uLayerCount", 3);

            uint count = 3 + 3 + 3; //This is how many floats are in a single vertex
            uint vertexCount = (uint)_chunk.GetMeshData().Length / count;

            _gl.DrawArrays(PrimitiveType.Triangles, 0, vertexCount);
        }

        private static unsafe void RenderLampCube()
        {
            _lampShader.Use();

            var lampMatrix = Matrix4x4.Identity;
            lampMatrix *= Matrix4x4.CreateScale(0.2f);
            lampMatrix *= Matrix4x4.CreateTranslation(_lampPosition);

            _lampShader.SetUniform("uModel", lampMatrix);
            _lampShader.SetUniform("uView", _camera.GetViewMatrix());
            _lampShader.SetUniform("uProjection", _camera.GetProjectionMatrix());

            _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }

        private static void OnClose()
        {
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            _blockShader.Dispose();
            _lampShader.Dispose();
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            if (key == Key.Escape)
            {
                _window.Close();
            }
        }

        private static unsafe void OnMouseMove(IMouse mouse, Vector2 position)
        {
            var lookSensitivity = 0.1f;

            if (_lastMousePosition == default)
            {
                _lastMousePosition = position;
                return;
            }

            var xOffset = (position.X - _lastMousePosition.X) * lookSensitivity;
            var yOffset = (position.Y - _lastMousePosition.Y) * lookSensitivity;
            _lastMousePosition = position;

            _camera.ModifyDirection(xOffset, yOffset);
        }
    }
}