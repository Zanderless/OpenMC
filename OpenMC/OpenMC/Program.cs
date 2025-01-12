using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using System.Drawing;
using Silk.NET.OpenAL;

namespace OpenMC
{
    class Program
    {

        private static IWindow _window;
        private static GL _gl;

        private static BufferObject<float> _vbo;
        private static BufferObject<uint> _ebo;
        private static VertexArrayObject<float, uint> _vao;

        private static Texture _texture;
        private static Shader _shader;

        private static float[] vertices =
        {
            //Vertex Position   UV Coords
             0.5f, 0.5f, 0.0f,  1.0f, 0.0f,
             0.5f, -0.5f, 0.0f, 1.0f, 1.0f,
            -0.5f, -0.5f, 0.0f, 0.0f, 1.0f,
            -0.5f, 0.5f, 0.5f,  0.0f, 0.0f
        };

        private static uint[] indicies =
        {
            0, 1, 3,
            1, 2, 3
        };

        public static void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 800),
                Title = "OpenMC - Indev"
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

            _ebo = new BufferObject<uint>(_gl, indicies, BufferTargetARB.ElementArrayBuffer);
            _vbo = new BufferObject<float>(_gl, vertices, BufferTargetARB.ArrayBuffer);
            _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);

            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            _vao.VertexAttributePointer(1, 2, VertexAttribPointerType.Float, 5, 3);

            _shader = new Shader(_gl, "shader.vert", "shader.frag");
            _texture = new Texture(_gl, "cobblestone.png");

            IInputContext input = _window.CreateInput();

            for (int i = 0; i < input.Keyboards.Count; i++)
                input.Keyboards[i].KeyDown += KeyDown;
        }

        private static void OnUpdate(double deltaTime)
        {

        }

        private static unsafe void OnRender(double deltaTime)
        {
            _gl.Clear(ClearBufferMask.ColorBufferBit);

            _vao.Bind();
            _shader.Use();

            _texture.Bind();

            _shader.SetUniform("uTexture", 0);

            _gl.DrawElements(PrimitiveType.Triangles, (uint)indicies.Length, DrawElementsType.UnsignedInt, null);
        }

        private static void OnFramebufferResize(Vector2D<int> newSize)
        {
            _gl.Viewport(newSize);
        }

        private static void OnClose()
        {
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            _shader.Dispose();
            _texture.Dispose();
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            if (key == Key.Escape)
            {
                _window.Close();
            }
        }
    }
}