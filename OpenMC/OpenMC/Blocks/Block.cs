using System.Numerics;

namespace OpenMC.Blocks
{
    public enum BlockType
    {
        air = 0,
        cobblestone = 1,
        dirt = 2,
    }

    public class Block
    {

        public string Name { get; set; }
        public int ID { get; set; }
        public bool IsTransparent { get; set; } = false;

        private Vector3 _position;
        private BlockType _blockType = BlockType.cobblestone;

        //Mesh
        private float[] _meshData =
        {
            //    Position               Normals                UVs               //Array Index

            //Front
            -0.5f, -0.5f,  0.5f,      0.0f,  0.0f,  1.0f,      0.0f, 0.0f,            0,//0
             0.5f,  0.5f,  0.5f,      0.0f,  0.0f,  1.0f,      1.0f, 1.0f,            0,//1
            -0.5f,  0.5f,  0.5f,      0.0f,  0.0f,  1.0f,      0.0f, 1.0f,            0,//2
                                                                                      
             0.5f, -0.5f,  0.5f,      0.0f,  0.0f,  1.0f,      1.0f, 0.0f,            0,//3
             0.5f,  0.5f,  0.5f,      0.0f,  0.0f,  1.0f,      1.0f, 1.0f,            0,//4
            -0.5f, -0.5f,  0.5f,      0.0f,  0.0f,  1.0f,      0.0f, 0.0f,            0,//5

            //Back
            -0.5f, -0.5f, -0.5f,      0.0f,  0.0f, -1.0f,      0.0f, 0.0f,            0,//0
            -0.5f,  0.5f, -0.5f,      0.0f,  0.0f, -1.0f,      0.0f, 1.0f,            0,//2
             0.5f,  0.5f, -0.5f,      0.0f,  0.0f, -1.0f,      1.0f, 1.0f,            0,//1

             0.5f, -0.5f, -0.5f,      0.0f,  0.0f, -1.0f,      1.0f, 0.0f,            0,//3
            -0.5f, -0.5f, -0.5f,      0.0f,  0.0f, -1.0f,      0.0f, 0.0f,            0, //5
             0.5f,  0.5f, -0.5f,      0.0f,  0.0f, -1.0f,      1.0f, 1.0f,            0, //4

             //Right
              0.5f, -0.5f, -0.5f,     1.0f,  0.0f,  0.0f,      0.0f, 0.0f,            0,
              0.5f,  0.5f, -0.5f,     1.0f,  0.0f,  0.0f,      0.0f, 1.0f,            0,
              0.5f,  0.5f,  0.5f,     1.0f,  0.0f,  0.0f,      1.0f, 1.0f,            0,

              0.5f, -0.5f,  0.5f,     1.0f,  0.0f,  0.0f,      1.0f, 0.0f,            0,
              0.5f, -0.5f, -0.5f,     1.0f,  0.0f,  0.0f,      0.0f, 0.0f,            0,
              0.5f,  0.5f,  0.5f,     1.0f,  0.0f,  0.0f,      1.0f, 1.0f,            0,

             //Left
             -0.5f, -0.5f, -0.5f,    -1.0f,  0.0f,  0.0f,      0.0f, 0.0f,            0,
             -0.5f,  0.5f,  0.5f,    -1.0f,  0.0f,  0.0f,      1.0f, 1.0f,            0,
             -0.5f,  0.5f, -0.5f,    -1.0f,  0.0f,  0.0f,      0.0f, 1.0f,            0,

             -0.5f, -0.5f,  0.5f,    -1.0f,  0.0f,  0.0f,      1.0f, 0.0f,            0,
             -0.5f,  0.5f,  0.5f,    -1.0f,  0.0f,  0.0f,      1.0f, 1.0f,            0,
             -0.5f, -0.5f, -0.5f,    -1.0f,  0.0f,  0.0f,      0.0f, 0.0f,            0,

             //Top
             -0.5f,  0.5f, -0.5f,     0.0f,  1.0f,  0.0f,      0.0f, 0.0f,            0,
              0.5f,  0.5f,  0.5f,     0.0f,  1.0f,  0.0f,      1.0f, 1.0f,            0,
              0.5f,  0.5f, -0.5f,     0.0f,  1.0f,  0.0f,      0.0f, 1.0f,            0,

             -0.5f,  0.5f,  0.5f,     0.0f,  1.0f,  0.0f,      1.0f, 0.0f,            0,
              0.5f,  0.5f,  0.5f,     0.0f,  1.0f,  0.0f,      1.0f, 1.0f,            0,
             -0.5f,  0.5f, -0.5f,     0.0f,  1.0f,  0.0f,      0.0f, 0.0f,            0,

             //Bottom
             -0.5f, -0.5f, -0.5f,     0.0f, -1.0f,  0.0f,      0.0f, 0.0f,            0,
              0.5f, -0.5f, -0.5f,     0.0f, -1.0f,  0.0f,      0.0f, 1.0f,            0,
              0.5f, -0.5f,  0.5f,     0.0f, -1.0f,  0.0f,      1.0f, 1.0f,            0,

             -0.5f, -0.5f,  0.5f,     0.0f, -1.0f,  0.0f,      1.0f, 0.0f,            0,
             -0.5f, -0.5f, -0.5f,     0.0f, -1.0f,  0.0f,      0.0f, 0.0f,            0,
              0.5f, -0.5f,  0.5f,     0.0f, -1.0f,  0.0f,      1.0f, 1.0f,            0,
        };

        private uint[] _indices =
        {
            0, 1, 2,
            3, 5, 4,
        };

        private void UpdateTextureIndex()
        {
            uint index = (uint)_blockType - 1;

            for(int i = 8; i < _meshData.Length; i += 9)
            {
                _meshData[i] = index;
            }
        }

        public Vector3 GetPosition()
        {
            return _position;
        }

        public float[] GetMeshData()
        {
            return _meshData;
        }

        public uint[] GetIndices()
        {
            return _indices;
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

            UpdateTextureIndex();
        }

    }
}
