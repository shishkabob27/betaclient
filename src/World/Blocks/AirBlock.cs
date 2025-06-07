

public class AirBlock : BlockDefinition
{
    public static readonly byte BlockID = 0x00;
    
    public override byte ID { get { return 0x00; } }

    public override double BlastResistance { get { return 0; } }

    public override double Hardness { get { return 0; } }

    public override byte Luminance { get { return 0; } }

    public override bool Opaque { get { return false; } }

    public override string GetDisplayName(short metadata)
    {
        return "Air";
    }

    public override BoundingBox BoundingBox { get { return new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero); } }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(0, 14);
    }
}