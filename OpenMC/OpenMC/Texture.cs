using System;
using System.IO;
using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using StbImageSharp;

namespace OpenMC
{
    public class Texture : IDisposable
    {

        private uint _handle;
        private GL _gl;

        public unsafe Texture(GL gl, string path)
        {
            _gl = gl;

            _handle = _gl.GenTexture();
            Bind(TextureTarget.Texture2D);

            string fPath = @"..\..\..\" + path;

            ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(fPath), ColorComponents.RedGreenBlueAlpha);

            fixed (byte* ptr = result.Data)
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)result.Width, (uint)result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
            }

            SetParameters(TextureTarget.Texture2D);
        }

        public unsafe Texture(GL gl, uint height, uint width)
        {
            _gl = gl;

            _handle = _gl.GenTexture();
            Bind(TextureTarget.Texture2DArray);

            string fPath = @"..\..\..\Textures";
            string[] files = Directory.GetFiles(fPath, "*.png");

            gl.TexStorage3D(GLEnum.Texture2DArray, 1, GLEnum.Rgba8, width, height, (uint)files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                ImageResult result = ImageResult.FromMemory(File.ReadAllBytes(files[i]), ColorComponents.RedGreenBlueAlpha);

                fixed (byte* ptr = result.Data)
                {
                    gl.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, i, width, height, 1, GLEnum.Rgba, GLEnum.UnsignedByte, ptr);
                }
            }

            SetParameters(TextureTarget.Texture2DArray);

        }

        public unsafe Texture(GL gl, Span<byte> data, uint width, uint height)
        {
            _gl = gl;

            _handle = _gl.GenTexture();
            Bind(TextureTarget.Texture2D);

            fixed(void* d = &data[0])
            {
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
                SetParameters(TextureTarget.Texture2D);
            }
        }

        private void SetParameters(TextureTarget target)
        {
            //Setting some texture perameters so the texture behaves as expected.
            _gl.TexParameter(target, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            _gl.TexParameter(target, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
            _gl.TexParameter(target, TextureParameterName.TextureMinFilter, (int)GLEnum.NearestMipmapNearest);
            _gl.TexParameter(target, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
            _gl.TexParameter(target, TextureParameterName.TextureBaseLevel, 0);
            _gl.TexParameter(target, TextureParameterName.TextureMaxLevel, 8);

            //Generating mipmaps.
            _gl.GenerateMipmap(target);
        }

        public void Bind(TextureTarget target, TextureUnit textureSlot = TextureUnit.Texture0, uint index = 0)
        {
            _gl.ActiveTexture(textureSlot + (int)index);
            _gl.BindTexture(target, _handle);
        }

        public void Dispose()
        {
            _gl.DeleteTexture(_handle);
        }
    }
}
