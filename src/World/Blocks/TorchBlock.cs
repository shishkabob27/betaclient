

public class TorchBlock : BlockDefinition
{
    public enum TorchDirection
    {
        East = 0x01,
        West = 0x02,
        South = 0x03,
        North = 0x04,
        Ground = 0x05
    }
    
    public override byte ID { get { return 0x32; } }
    
    public override double BlastResistance { get { return 0; } }

    public override double Hardness { get { return 0; } }

    public override byte Luminance { get { return 13; } }

    public override bool Opaque { get { return false; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Torch";
    }

    public override BoundingBox BoundingBox { get { return new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.Zero); } }

    public override BoundingBox InteractiveBoundingBox
    {
        get
        {
            return new BoundingBox(new System.Numerics.Vector3((float)(4 / 16.0), 0, (float)(4 / 16.0)), new System.Numerics.Vector3((float)(12 / 16.0), (float)(7.0 / 16.0), (float)(12 / 16.0)));
        }
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(0, 5);
    }
}