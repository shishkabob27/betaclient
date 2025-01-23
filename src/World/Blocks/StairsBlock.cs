public class WoodenStairsBlock : BlockDefinition
{
    public override byte ID { get { return 0x35; } }
    
    public override double BlastResistance { get { return 15; } }

    public override double Hardness { get { return 0; } }

    public override byte Luminance { get { return 0; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Wooden Stairs";
    }
}

public class StoneStairsBlock : BlockDefinition
{
    public override byte ID { get { return 0x43; } }

    public override double BlastResistance { get { return 30; } }

    public override double Hardness { get { return 0; } }

    public override byte Luminance { get { return 0; } }

    public override string GetDisplayName(short metadata)
    {
        return "Stone Stairs";
    }
}