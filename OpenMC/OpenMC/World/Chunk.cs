using System.Drawing;
using System.Numerics;
using OpenMC.Blocks;
using OpenMC.Rendering;

namespace OpenMC.World
{
    public class Chunk
    {

        public Mesh mesh;
        public Transform transform;

        private Vector3 _size;
        private Vector2 _position;

        private Block[,,] _blocks;

        private float[] _meshData;

        private uint[] _indices =
        {
            0, 1, 2,
            3, 4, 5,
        };

        public Chunk(Vector3 size, Vector2 chunkPosition)
        {
            mesh = new Mesh(Program._gl);

            transform = new Transform();//Create our transform
            transform.Position = WorldManager.ChunkToWorldCoordinates(chunkPosition);
            _position = chunkPosition;

            _size = new Vector3((uint)size.X, (uint)size.Y, (uint)size.Z);
            CreateVoxelGrid();
            GenerateMeshData();
        }

        private void CreateVoxelGrid()
        {
            _blocks = new Block[(uint)_size.X, (uint)_size.Y, (uint)_size.Z];

            for (int z = 0; z < _size.Z; z++)
            {
                for (int y = 0; y < _size.Y; y++)
                {
                    for (int x = 0; x < _size.X; x++)
                    {
                        Vector3 cPos = WorldManager.ChunkToWorldCoordinates(_position);
                        Vector2 pos = new Vector2(x + cPos.X, z + cPos.Z);

                        float noiseLevel = WorldGeneration.SampleNoise((int)pos.X, (int)pos.Y);
                        noiseLevel *= _size.Y;

                        Block b;

                        if (y == MathF.Floor(noiseLevel))
                            b = new Block(BlockType.dirt, new Vector3(x, y, z));
                        else if(y < noiseLevel)
                            b = new Block(BlockType.cobblestone, new Vector3(x, y, z));
                        else
                        b = new Block(BlockType.air, new Vector3(x, y, z));

                        _blocks[x, y, z] = b;
                    }
                }
            }
        }

        private void GenerateMeshData()
        {
            List<float> meshData = new List<float>();

            for (int z = 0; z < _size.Z; z++)
            {
                for (int y = 0; y < _size.Y; y++)
                {
                    for (int x = 0; x < _size.X; x++)
                    {
                        Block b = _blocks[x, y, z];

                        if (b.GetBlockType() == BlockType.air || b == null)
                            continue;

                        for (int i = 0; i < b.GetMeshData().Length;)
                        {
                            var normal = new Vector3(b.GetMeshData()[i + 3], b.GetMeshData()[i + 4], b.GetMeshData()[i + 5]);
                            var checkPos = normal + b.GetPosition();

                            if (!IsBlockAir(checkPos))
                            {
                                i += 9; //Add this so we aren't checking the same code over and over again
                                continue;
                            }

                            //Position
                            meshData.Add(b.GetMeshData()[i] + x); //X
                            meshData.Add(b.GetMeshData()[i + 1] + y); //Y
                            meshData.Add(b.GetMeshData()[i + 2] + z); //Z

                            //Normals
                            meshData.Add(b.GetMeshData()[i + 3]);
                            meshData.Add(b.GetMeshData()[i + 4]);
                            meshData.Add(b.GetMeshData()[i + 5]);

                            //UVs
                            meshData.Add(b.GetMeshData()[i + 6]);
                            meshData.Add(b.GetMeshData()[i + 7]);
                            meshData.Add(b.GetMeshData()[i + 8]); //Texture Index

                            i += 9;
                        }
                    }
                }
            }

            _meshData = meshData.ToArray();
        }

        public float[] GetMeshData()
        {
            return _meshData;
        }

        public uint[] GetIndices()
        {
            return _indices;
        }

        public bool IsBlockAir(Vector3 localPosition)
        {
            if (localPosition.X < 0 || localPosition.X >= _size.X
                || localPosition.Y < 0 || localPosition.Y >= _size.Y
                || localPosition.Z < 0 || localPosition.Z >= _size.Z)
                return true;

            return _blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z].GetBlockType() == BlockType.air;
        }

    }
}
