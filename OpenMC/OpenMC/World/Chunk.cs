using System.Numerics;
using OpenMC.Blocks;

namespace OpenMC.World
{
    public class Chunk
    {
        public Vector3 Size { get; private set; }

        private Block[,,] _blocks;

        private float[] _meshData;

        private uint[] _indices =
        {
            0, 1, 2,
            3, 4, 5,
        };

        public Chunk(int sizeX, int sizeY, int sizeZ)
        {
            Size = new Vector3(sizeX, sizeY, sizeZ);
            GenerateChunk();
        }

        public void GenerateChunk()
        {
            //Generate Voxel Grid and fill it with blocks
            CreateVoxelGrid();

            //Once the voxel grid is created, we'll grab and save the mesh data into an array
            CreateMeshData();
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
            if (localPosition.X < 0 || localPosition.X >= Size.X
                || localPosition.Y < 0 || localPosition.Y >= Size.Y
                || localPosition.Z < 0 || localPosition.Z >= Size.Z)
                return true;

            return _blocks[(int)localPosition.X, (int)localPosition.Y, (int)localPosition.Z].GetBlockType() == BlockType.air;
        }

        private void CreateVoxelGrid()
        {
            _blocks = new Block[(uint)Size.X, (uint)Size.Y, (uint)Size.Z];

            for (int z = 0; z < Size.Z; z++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    for (int x = 0; x < Size.X; x++)
                    {
                        Block b;

                        if (y == Size.Y - 1)
                            b = new Block(BlockType.dirt, new Vector3(x, y, z));
                        else
                        {
                            Random random = new Random();
                            int r = random.Next(100);
                            b = new Block(BlockType.cobblestone, new Vector3(x, y, z));
                        }

                        _blocks[x, y, z] = b;
                    }
                }
            }
        }

        private void CreateMeshData()
        {
            List<float> meshData = new List<float>();

            for (int z = 0; z < Size.Z; z++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    for (int x = 0; x < Size.X; x++)
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

    }
}
