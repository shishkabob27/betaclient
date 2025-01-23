public class NetherrackBlock : BlockDefinition
{
    public override byte ID { get { return 0x57; } }
    
    public override double BlastResistance { get { return 2; } }

    public override double Hardness { get { return 0.4; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Netherrack";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(7, 6);
    }
}