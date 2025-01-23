public class WoodenPlanksBlock : BlockDefinition
{    
    public override byte ID { get { return 0x05; } }
    
    public override double BlastResistance { get { return 15; } }

    public override double Hardness { get { return 2; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Wooden Planks";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(4, 0);
    }
}