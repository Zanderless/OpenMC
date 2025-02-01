
namespace OpenMC.Noise
{
    public abstract class NoiseProfile
    {
        public abstract int NoiseType { get;}
        public abstract float Frequency { get;}
        public abstract int FractalNoise { get; }
        public abstract int Octaves { get; }
        public abstract float Lacunarity { get; }
        public abstract float Gain { get; }
    }

    public class Terrain_1_Profile : NoiseProfile
    {
        public override int NoiseType => 4;
        public override float Frequency => 0.007f;

        public override int FractalNoise => 0;
        public override int Octaves => 4;

        public override float Lacunarity => 1.7f;

        public override float Gain => 0.5f;
    }

    public class Terrain_2_Profile : NoiseProfile
    {
        public override int NoiseType => 4;
        public override float Frequency => 0.013f;

        public override int FractalNoise => 0;
        public override int Octaves => 5;

        public override float Lacunarity => 2f;

        public override float Gain => 0.75f;
    }
}
