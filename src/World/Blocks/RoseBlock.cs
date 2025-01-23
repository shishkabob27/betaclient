using Raylib_cs;

public class RoseBlock : BlockDefinition
{
    public override byte ID { get { return 38; } }
    
    public override double BlastResistance { get { return 0; } }

    public override double Hardness { get { return 0; } }

    public override byte Luminance { get { return 0; } }

    public override bool Opaque { get { return false; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Rose";
    }

    public override BoundingBox BoundingBox { get { return new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero); } }

    public override BoundingBox InteractiveBoundingBox
    {
        get
        {
            return new BoundingBox(new System.Numerics.Vector3(4f / 16.0f, 0, 4f / 16.0f), new System.Numerics.Vector3(12f / 16.0f, 8f / 16.0f, 12f / 16.0f));
        }
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(12, 0);
    }
}