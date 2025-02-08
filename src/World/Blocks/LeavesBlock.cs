public class LeavesBlock : BlockDefinition
{
    public static readonly byte BlockID = 0x12;
    
    public override byte ID { get { return 0x12; } }
    
    public override double BlastResistance { get { return 1; } }

    public override double Hardness { get { return 0.2; } }

    public override bool Opaque { get { return false; } }

    public override string GetDisplayName(short metadata)
    {
        return "Leaves";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(5, 3);
    }
}