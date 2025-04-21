using System.Diagnostics;

public class ChestBlock : BlockDefinition
{    
    public override byte ID { get { return 0x36; } }
    
    public override double BlastResistance { get { return 12.5; } }

    public override double Hardness { get { return 2.5; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Chest";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(10, 1);
    }
}