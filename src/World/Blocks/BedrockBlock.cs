public class BedrockBlock : BlockDefinition
{
    public static readonly byte BlockID = 0x07;
    
    public override byte ID { get { return 0x07; } }

    public override double BlastResistance { get { return 18000000; } }

    public override double Hardness { get { return -1; } }

    public override byte Luminance { get { return 0; } }

    public override string GetDisplayName(short metadata)
    {
        return "Bedrock";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(1, 1);
    }
}