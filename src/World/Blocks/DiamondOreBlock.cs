public class DiamondOreBlock : BlockDefinition
{    
    public override byte ID { get { return 56; } }
    
    public override double BlastResistance { get { return 15; } }

    public override double Hardness { get { return 3; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Diamond Ore";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(2, 3);
    }
}