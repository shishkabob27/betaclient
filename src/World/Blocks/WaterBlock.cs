
using Raylib_cs;

public class WaterBlock : BlockDefinition
{
    public override byte ID { get { return 8; } }
    
    public override double BlastResistance { get { return 500; } }

    public override double Hardness { get { return 100; } }

    public override byte Luminance { get { return 0; } }

    public override bool Opaque { get { return false; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Water";
    }

    public override BoundingBox BoundingBox { get { return new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero); } }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(13, 12);
    }
}

public class StationaryWaterBlock : WaterBlock
{
    public override byte ID { get { return 9; } }

    public override string GetDisplayName(short metadata)
    {
        return "Water (stationary)";
    }
}