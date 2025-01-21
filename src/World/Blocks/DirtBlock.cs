public class DirtBlock : BlockDefinition
{
    public static readonly byte BlockID = 0x03;
    
    public override byte ID { get { return 0x03; } }
    
    public override double BlastResistance { get { return 2.5; } }

    public override double Hardness { get { return 0.5; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Dirt";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(2, 0);
    }
}