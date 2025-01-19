using System.Numerics;

namespace OpenMC.Blocks
{
    public enum BlockType
    {
        cobblestone = 1
    }

    public class Block
    {

        private Vector3 _position;

        //Block Information
        private BlockType _blockType = BlockType.cobblestone;

        //Mesh
        private float[] _meshData =
        {
            -0.5f, -0.5f, -0.5f,   0.0f, 0.0f, -1.0f,   0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,   0.0f, 0.0f, -1.0f,   1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,   0.0f, 0.0f, -1.0f,   1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,   0.0f, 0.0f, -1.0f,   1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,   0.0f, 0.0f, -1.0f,   0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,   0.0f, 0.0f, -1.0f,   0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,   0.0f, 0.0f, 1.0f,    0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,   0.0f, 0.0f, 1.0f,    1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,    1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,    1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,    0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,   0.0f, 0.0f, 1.0f,    0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  -1.0f, 0.0f, 0.0f,    1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  -1.0f, 0.0f, 0.0f,    1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  -1.0f, 0.0f, 0.0f,    0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  -1.0f, 0.0f, 0.0f,    0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  -1.0f, 0.0f, 0.0f,    0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  -1.0f, 0.0f, 0.0f,    1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,   1.0f, 0.0f, 0.0f,    1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,   1.0f, 0.0f, 0.0f,    1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,   1.0f, 0.0f, 0.0f,    0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,   1.0f, 0.0f, 0.0f,    0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,   1.0f, 0.0f, 0.0f,    0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,   1.0f, 0.0f, 0.0f,    1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,   0.0f, -1.0f, 0.0f,   0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,   0.0f, -1.0f, 0.0f,   1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,   0.0f, -1.0f, 0.0f,   1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,   0.0f, -1.0f, 0.0f,   1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,   0.0f, -1.0f, 0.0f,   0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,   0.0f, -1.0f, 0.0f,   0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,   0.0f, 1.0f, 0.0f,    0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,   0.0f, 1.0f, 0.0f,    1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,   0.0f, 1.0f, 0.0f,    1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,   0.0f, 1.0f, 0.0f,    1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,   0.0f, 1.0f, 0.0f,    0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,   0.0f, 1.0f, 0.0f,    0.0f, 1.0f
        };

        private uint[] _indicies =
        {
            0, 1, 3,
            1, 2, 3
        };

        public Vector3 GetPosition()
        {
            return _position;
        }

        public float[] GetMeshData()
        {
            return _meshData;
        }

        public uint[] GetIndicies()
        {
            return _indicies;
        }

        public void SetBlockType(BlockType type)
        {
            _blockType = type;
        }

        public BlockType GetBlockType()
        {
            return _blockType;
        }

        public Block(BlockType type, Vector3 position)
        {
            _blockType = type;
            _position = position;
        }

    }
}
