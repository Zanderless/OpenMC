using OpenMC.Noise;

namespace OpenMC.World
{

    public static class WorldGeneration
    {

        static FastNoise _noise;

        public static void Initalize()
        {
            _noise = new FastNoise();
        }

        public static void GenerateNoiseMap(int seed, NoiseProfile profile)
        { 
            _noise.SetSeed(seed);
            _noise.SetNoiseType((FastNoise.NoiseType)profile.NoiseType);
            _noise.SetFrequency(profile.Frequency);
            _noise.SetFractalType((FastNoise.FractalType)profile.FractalNoise);
            _noise.SetFractalOctaves(profile.Octaves);
            _noise.SetFractalLacunarity(profile.Lacunarity);
            _noise.SetFractalGain(profile.Gain);
        }

        public static float SampleNoise(int x, int z)
        {
            return (_noise.GetNoise(x, z) + 1.0f) / 2.0f;
        }
    }
}
