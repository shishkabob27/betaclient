using Raylib_cs;

public class TallGrassBlock : BlockDefinition
{
    public enum TallGrassType
    {
        DeadBush = 0,
        TallGrass = 1,
        Fern = 2
    }

    public static readonly byte BlockID = 0x1F;
    
    public override byte ID { get { return 0x1F; } }
    
    public override double BlastResistance { get { return 0; } }

    public override double Hardness { get { return 0; } }

    public override byte Luminance { get { return 0; } }

    public override bool Opaque { get { return false; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Tall Grass";
    }

    public override BoundingBox BoundingBox { get { return new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero); } }

    public override BoundingBox InteractiveBoundingBox
    {
        get
        {
            return new BoundingBox(new System.Numerics.Vector3(4f / 16.0f), System.Numerics.Vector3.One);
        }
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(7, 2);
    }
}