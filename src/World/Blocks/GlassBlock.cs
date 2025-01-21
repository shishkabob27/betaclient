public class GlassBlock : BlockDefinition
{   
    public override byte ID { get { return 20; } }
    
    public override double BlastResistance { get { return 1.5; } }

    public override double Hardness { get { return 0.3; } }

    public override byte Luminance { get { return 0; } }

    public override bool Opaque { get { return false; } }
    
    public override string GetDisplayName(short metadata)
    {
        return "Glass";
    }

    public override Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
    {
        return new Tuple<int, int>(1, 3);
    }
}