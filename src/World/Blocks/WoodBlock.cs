public class WoodBlock : BlockDefinition
{
    public enum WoodType
    {
        Oak = 0,
        Spruce = 1,
        Birch = 2
    }
    
    public override byte ID { get { return 0x11; } }
    
    public override double BlastResistance { get { return 10; } }

    public override double Hardness { get { return 2; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Wood";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(4, 1);
    }
}