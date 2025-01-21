public class GrassBlock : BlockDefinition
{
    public static readonly byte BlockID = 0x02;
    
    public override byte ID { get { return 0x02; } }
    
    public override double BlastResistance { get { return 3; } }

    public override double Hardness { get { return 0.6; } }

    public override byte Luminance { get { return 0; } }

    public override string GetDisplayName(short metadata)
    {
        return "Grass";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        if (face == BlockFace.PositiveY)
        {
            return new Tuple<int, int>(0, 0);
        }
        else if (face == BlockFace.NegativeY)
        {
            return new Tuple<int, int>(2, 0);
        }
        else
        {
            return new Tuple<int, int>(3, 0);
        }
    }
}