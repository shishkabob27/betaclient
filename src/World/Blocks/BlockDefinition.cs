using System;
using System.Numerics;
using Raylib_cs;

public class BlockDefinition
{
	public virtual byte ID { get { return 0; } }
	
	public virtual double BlastResistance { get { return 0; } }

	public virtual double Hardness { get { return 0; } }

	public virtual byte Luminance { get { return 0; } }

	public virtual bool Opaque { get { return true; } }

	public virtual BoundingBox BoundingBox { get { return new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.One); } }
	public virtual BoundingBox InteractiveBoundingBox { get { return new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.One); } }
	
	public virtual string GetDisplayName(short metadata)
	{
		return string.Empty;
	}

	public virtual Tuple<int, int> GetTextureMap(byte metadata, BlockFace face)
	{
		return new Tuple<int, int>(0, 14);
	}
}