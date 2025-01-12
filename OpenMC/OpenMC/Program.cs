using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using System.Drawing;
using StbImageSharp;
using Silk.NET.Vulkan;

public class Program
{

    private static uint _program; //Shader Program

    private static uint _vao; //Vertex Array Object
    private static uint _vbo; //Vertext Buffer Object
    private static uint _ebo; //Element Buffer Object

    private static uint _texture;

    private static IWindow _window;
    private static GL _gl;

    public static void Main(string[] args)
    {
        WindowOptions options = WindowOptions.Default with
        {
            Size = new Vector2D<int>(800, 600),
            Title = "OpenMC - Indev"
        };

        _window = Window.Create(options);

        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;

        _window.Run();

        _window.Dispose();
    }

    private static unsafe void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _gl.ClearColor(Color.CornflowerBlue);

        _vao = _gl.CreateVertexArray();
        _gl.BindVertexArray(_vao);

        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);

        float[] verticies =
        {
            //   aPosition       aTexCoords
             0.5f,  0.5f, 0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, 0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,  0.0f, 1.0f
        };

        fixed (float* buf = verticies)
            _gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticies.Length * sizeof(float)), buf, BufferUsageARB.StaticDraw);

        uint[] indicies =
        {
            0u, 1u, 3u,
            1u, 2u, 3u
        };

        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);

        fixed(uint* buf = indicies)
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicies.Length * sizeof(uint)), buf, BufferUsageARB.StaticDraw);

        //Ran on every vertex of the mesh
        const string vertCode = @"
        #version 330 core
        
        layout (location = 0) in vec3 aPosition;
        layout (location = 1) in vec2 aTexCoords;

        out vec2 frag_texCoords;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
            frag_texCoords = aTexCoords;
        }
        ";

        //Ran on every pixel of the mesh
        const string fragmentCode = @"
        #version 330 core
        
        in vec2 frag_texCoords;

        out vec4 out_color;

        uniform sampler2D uTexture;

        void main()
        {
            out_color = texture(uTexture, frag_texCoords);
        }
        ";

        //Vertex Shader
        uint vertShader = _gl.CreateShader(ShaderType.VertexShader); //Create the shader
        _gl.ShaderSource(vertShader, vertCode); //Bind the source code to the shader

        _gl.CompileShader(vertShader);

        _gl.GetShader(vertShader, ShaderParameterName.CompileStatus, out int vStatus);
        if(vStatus != (int)GLEnum.True)
            throw new Exception("Vertex shader failed to compile: " + _gl.GetShaderInfoLog(vertShader));

        //Fragment Shader
        uint fragmentShader = _gl.CreateShader(ShaderType.FragmentShader); //Create the shader
        _gl.ShaderSource(fragmentShader, fragmentCode); //Bind the source code to the shader

        _gl.CompileShader(fragmentShader);

        _gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out int fStatus);
        if (vStatus != (int)GLEnum.True)
            throw new Exception("Fragment shader failed to compile: " + _gl.GetShaderInfoLog(fragmentShader));

        _program = _gl.CreateProgram();

        _gl.AttachShader(_program, vertShader);
        _gl.AttachShader(_program, fragmentShader);

        _gl.LinkProgram(_program);

        _gl.GetProgram(_program, ProgramPropertyARB.LinkStatus, out int lStatus);
        if (lStatus != (int)GLEnum.True)
            throw new Exception("Program failed to link: " + _gl.GetProgramInfoLog(_program));

        _gl.DetachShader(_program, vertShader);
        _gl.DetachShader(_program, fragmentShader);
        _gl.DeleteShader(vertShader);
        _gl.DeleteShader(fragmentShader);

        const uint positionLoc = 0;
        _gl.EnableVertexAttribArray(positionLoc);
        _gl.VertexAttribPointer(positionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)0); //We start at 0 and we read the first 3 elements before skipping ahead a remaining 2

        const uint texCoordLoc = 1;
        _gl.EnableVertexAttribArray(texCoordLoc);
        _gl.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), (void*)(3 * sizeof(float))); //We start at 2 and read the first 2 elements before skipping ahead a remaining 3

        _gl.BindVertexArray(0); //We need to bind this first or else we'll get errors
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

        _texture = _gl.GenTexture();
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _texture);

        ImageResult result = ImageResult.FromMemory(File.ReadAllBytes("Textures/cobblestone.png"), ColorComponents.RedGreenBlueAlpha);

        fixed(byte* ptr = result.Data)
            _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)result.Width, (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);

        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, (int)TextureWrapMode.Repeat);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)TextureMagFilter.Nearest);

        _gl.GenerateMipmap(TextureTarget.Texture2D);

        _gl.BindTexture(TextureTarget.Texture2D, 0);

        int location = _gl.GetUniformLocation(_program, "uTexture");
        _gl.Uniform1(location, 0);

        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

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

        _gl.BindVertexArray(_vao);
        _gl.UseProgram(_program);
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, _texture);
        _gl.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (void*)0);
    }

    private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
    {
        if(key == Key.Escape)
        {
            _window.Close();
        }
    }
}