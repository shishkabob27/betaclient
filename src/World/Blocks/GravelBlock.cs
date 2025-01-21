public class GravelBlock : BlockDefinition
{
    public static readonly byte BlockID = 0x0D;
    
    public override byte ID { get { return 0x0D; } }
    
    public override double BlastResistance { get { return 3; } }

    public override double Hardness { get { return 0.6; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Gravel";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(3, 1);
    }
}