using OpenMC.Noise;
using OpenMC.Rendering;
using Silk.NET.OpenGL;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace OpenMC.World
{
    public class WorldManager
    {

        public Chunk[,] _renderedChunks;

        public static Vector3 chunkSize { get; private set; } = new Vector3(16, 128, 16);

        public unsafe void OnRender()
        {
            foreach (var chunk in _renderedChunks)
            {
                uint vertexCount = (uint)chunk.GetMeshData().Length / 9; //9 is the length of a data set in meshData
                chunk.mesh.RenderMesh(chunk.transform, vertexCount);
            }
        }

        public void CreateWorld(int seed)
        {
            //Create the world and grab its basic information
            _renderedChunks = new Chunk[5, 5];

            WorldGeneration.GenerateNoiseMap(seed, new Terrain_1_Profile());

            for (int y = 0; y < _renderedChunks.GetLength(1); y++)
            {
                for (int x = 0; x < _renderedChunks.GetLength(0); x++)
                {
                    _renderedChunks[x,y] = new Chunk(chunkSize, new Vector2(x, y));

                    _renderedChunks[x, y].mesh.SetMeshData(_renderedChunks[x, y].GetMeshData(), _renderedChunks[x, y].GetIndices());
                    _renderedChunks[x, y].mesh.SetShader("shader.vert", "block.frag");

                    _renderedChunks[x, y].mesh.CreateMesh();
                }
            }
        }

        public void Dispose()
        {
            foreach(Chunk chunk in _renderedChunks)
            {
                chunk.mesh.Dispose();
            }
        }

        //Converts the positions of chunks to world coordinates (vec2 -> vec3)
        public static Vector3 ChunkToWorldCoordinates(Vector2 chunkPosition)
        {
            int xPos = (int)chunkPosition.X * (int)chunkSize.X;
            int ZPos = (int)chunkPosition.Y * (int)chunkSize.Z;

            return new Vector3(xPos, 0, ZPos);
        }

        public WorldManager()
        {
            WorldGeneration.Initalize();
        }

    }
}
