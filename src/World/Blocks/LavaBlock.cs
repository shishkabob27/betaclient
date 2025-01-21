using Raylib_cs;

public class LavaBlock : BlockDefinition
{
    public override byte ID { get { return 10; } }

    public override double BlastResistance { get { return 0; } }

    public override double Hardness { get { return 100; } }

    public override byte Luminance { get { return 15; } }

    public override bool Opaque { get { return false; } }

    public override string GetDisplayName(short metadata)
    {
        return "Lava";
    }

    public override BoundingBox BoundingBox { get { return new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero); } }
}

public class StationaryLavaBlock : LavaBlock
{   
    public override byte ID { get { return 11; } }
    
    public override double BlastResistance { get { return 500; } }

    public override string GetDisplayName(short metadata)
    {
        return "Lava (stationary)";
    }
}