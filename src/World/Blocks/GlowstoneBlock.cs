public class GlowstoneBlock : BlockDefinition
{
    public override byte ID { get { return 0x59; } }
    
    public override double BlastResistance { get { return 1.5; } }

    public override double Hardness { get { return 0.3; } }

    public override byte Luminance { get { return 15; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Glowstone";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(9, 6);
    }
}