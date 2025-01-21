public class CobblestoneBlock : BlockDefinition
{
    public static readonly byte BlockID = 0x04;
    
    public override byte ID { get { return 0x04; } }
    
    public override double BlastResistance { get { return 30; } }

    public override double Hardness { get { return 2; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Cobblestone";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(0, 1);
    }
}