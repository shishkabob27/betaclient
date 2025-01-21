public class StairsBlock : BlockDefinition
{
    public enum StairDirection
    {
        East = 0,
        West = 1,
        South = 2,
        North = 3
    }

    public override double Hardness { get { return 0; } }

    public override byte Luminance { get { return 0; } }
}

public class WoodenStairsBlock : StairsBlock
{
    public static readonly byte BlockID = 0x35;
    
    public override byte ID { get { return 0x35; } }
    
    public override double BlastResistance { get { return 15; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Wooden Stairs";
    }
}

public class StoneStairsBlock : StairsBlock
{
    public static readonly byte BlockID = 0x43;

    public override byte ID { get { return 0x43; } }

    public override double BlastResistance { get { return 30; } }

    public override string GetDisplayName(short metadata)
    {
        return "Stone Stairs";
    }
}