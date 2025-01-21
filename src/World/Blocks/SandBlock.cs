public class SandBlock : BlockDefinition
{        
    public override byte ID { get { return 12; } }
    
    public override double BlastResistance { get { return 2.5; } }

    public override double Hardness { get { return 0.5; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Sand";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(2, 1);
    }
}