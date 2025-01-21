
public class ObsidianBlock : BlockDefinition
{   
    public override byte ID { get { return 49; } }
    
    public override double BlastResistance { get { return 6000; } }

    public override double Hardness { get { return 10; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Obsidian";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(5, 2);
    }
}