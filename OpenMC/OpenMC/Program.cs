using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using System.Drawing;
using System.Numerics;
using OpenMC.Blocks;
using OpenMC.World;
using OpenMC.Rendering;

namespace OpenMC
{
    class Program
    {

        private static GL _gl;
        private static IWindow _window;
        private static IKeyboard _keyboard;
        private static Renderer _renderer;

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

            _chunk = new Chunk(16, 128, 16);

            var size = _window.FramebufferSize;

            Vector3 spawnPos = new Vector3(_chunk.Size.X / 2, _chunk.Size.Y + 2, _chunk.Size.Z / 2);
            //Vector3 spawnPos = new Vector3(0, 0, 5);

            _camera = new Camera(spawnPos, Vector3.UnitZ * -1, Vector3.UnitY, (float)size.X / size.Y);

            _renderer = new Renderer(_gl, _camera);

            _renderer.SetMeshData(_chunk.GetMeshData(), _chunk.GetIndices());
            _renderer.SetShader("shader.vert", "block.frag");

            _renderer.CreateMesh();

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

            if (_keyboard.IsKeyPressed(Key.Space))
                _camera.Position += moveSpeed * _camera.Up;
            else if(_keyboard.IsKeyPressed(Key.ShiftLeft))
                _camera.Position -= moveSpeed * _camera.Up;
        }

        private static unsafe void OnRender(double deltaTime)
        {
            _gl.Enable(EnableCap.DepthTest);
            _gl.Clear((uint)(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

            uint vertexCount = (uint)_chunk.GetMeshData().Length / 9; //9 is the length of a data set in meshData
            _renderer.RenderMesh(vertexCount);
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            _gl.Viewport(newSize);
            _camera.AspectRatio = (float)newSize.X / newSize.Y;
        }

        private static void OnClose()
        {
            _renderer.Dispose();
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