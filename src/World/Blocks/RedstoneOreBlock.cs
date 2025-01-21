
public class RedstoneOreBlock : BlockDefinition
{
    public override byte ID { get { return 73; } }

    public override double BlastResistance { get { return 15; } }

    public override double Hardness { get { return 3; } }

    public override byte Luminance { get { return 0; } }

    public override string GetDisplayName(short metadata)
    {
        return "Redstone Ore";
    }


    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(3, 3);
    }
}

public class GlowingRedstoneOreBlock : RedstoneOreBlock
{    
    public override byte ID { get { return 74; } }

    public override byte Luminance { get { return 9; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Redstone Ore (glowing)";
    }
}