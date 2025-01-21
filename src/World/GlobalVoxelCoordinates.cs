/// <summary>
/// Specifies the location of a Voxel in 3D Global Coordinates.
/// </summary>
/// <remarks>
///<para>
/// These coordinates specify the number of Blocks/Voxels away from the origin.
///</para>
/// </remarks>
public class GlobalVoxelCoordinates : IEquatable<GlobalVoxelCoordinates>
{
	/// <summary>
	/// The X component of the coordinates.
	/// </summary>
	public int X { get; }

	/// <summary>
	/// The Y component of the coordinates.
	/// </summary>
	public int Y { get; }

	/// <summary>
	/// The Z component of the coordinates.
	/// </summary>
	public int Z { get; }

	/// <summary>
	/// Creates a new trio of coordinates from the specified values.
	/// </summary>
	/// <param name="x">The X component of the coordinates.</param>
	/// <param name="z">The Y component of the coordinates.</param>
	/// <param name="z">The Z component of the coordinates.</param>
	public GlobalVoxelCoordinates(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	/// <summary>
	/// Creates a new instance of GlobalVoxelCoordinates that is a copy of the given one.
	/// </summary>
	/// <param name="other"></param>
	public GlobalVoxelCoordinates(GlobalVoxelCoordinates other)
	{
		X = other.X;
		Y = other.Y;
		Z = other.Z;
	}

	/// <summary>
	/// Gets the Global Voxel Coordinates specified by the local coordinates within
	/// the given chunk coordinates.
	/// </summary>
	/// <param name="chunk">The coordinates of the Chunk.</param>
	/// <param name="local">The Local Voxel Coordinates within the Chunk.</param>
	public static GlobalVoxelCoordinates GetGlobalVoxelCoordinates(GlobalChunkCoordinates chunk, LocalVoxelCoordinates local)
	{
		int x = chunk.X * WorldConstants.ChunkWidth + local.X;
		int z = chunk.Z * WorldConstants.ChunkDepth + local.Z;

		return new GlobalVoxelCoordinates(x, local.Y, z);
	}

	/// <summary>
	/// Converts this GlobalVoxelCoordinates to a string in the format &lt;x, y, z&gt;.
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return $"<{X},{Y},{Z}>";
	}

	#region Math
	/// <summary>
	/// Calculates the distance between two GlobalVoxelCoordinates objects.
	/// </summary>
	public double DistanceTo(GlobalVoxelCoordinates other)
	{
		return Math.Sqrt(Square(other.X - X) +
						Square(other.Y - Y) +
						Square(other.Z - Z));
	}

	/// <summary>
	/// Calculates the distance from the center of this Voxel to the
	/// given position vector.
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public double DistanceTo(Vector3 other)
	{
		double dx = other.X - (this.X + 0.5);
		double dy = other.Y - (this.Y + 0.5);
		double dz = other.Z - (this.Z + 0.5);
		return Math.Sqrt(dx * dx + dy * dy + dz * dz);
	}

	/// <summary>
	/// Calculates the square of a num.
	/// </summary>
	private int Square(int num)
	{
		return num * num;
	}

	/// <summary>
	/// Finds the distance of this Coordinate3D from GlobalVoxelCoordinates.Zero
	/// </summary>
	public double Distance
	{
		get
		{
			return DistanceTo(Zero);
		}
	}
	#endregion

	#region Operators

	public static bool operator !=(GlobalVoxelCoordinates? a, GlobalVoxelCoordinates? b)
	{
		return !(a == b);
	}

	public static bool operator ==(GlobalVoxelCoordinates? a, GlobalVoxelCoordinates? b)
	{
		if (a is null)
		{
			if (b is null)
				return true;
			else
				return false;
		}
		else
		{
			if (b is null)
				return false;
			else
				return a.Equals(b);
		}
	}

	public GlobalVoxelCoordinates Add(Vector3i other)
	{
		return new GlobalVoxelCoordinates(this.X + other.X, this.Y + other.Y, this.Z + other.Z);
	}

	public static GlobalVoxelCoordinates operator+(GlobalVoxelCoordinates c, Vector3i v)
	{
		return c.Add(v);
	}

	public static GlobalVoxelCoordinates operator +(Vector3i v, GlobalVoxelCoordinates c)
	{
		return c.Add(v);
	}

	public static Vector3i operator-(GlobalVoxelCoordinates l, GlobalVoxelCoordinates r)
	{
		return new Vector3i(l.X - r.X, l.Y - r.Y, l.Z - r.Z);
	}

	public static GlobalVoxelCoordinates operator-(GlobalVoxelCoordinates arg)
	{
		return new GlobalVoxelCoordinates(-arg.X, -arg.Y, -arg.Z);
	}
	#endregion

	#region Conversion operators

	public static explicit operator GlobalVoxelCoordinates(GlobalColumnCoordinates a)
	{
		return new GlobalVoxelCoordinates(a.X, 0, a.Z);
	}

	public static explicit operator GlobalVoxelCoordinates(Vector3 a)
	{
		return new GlobalVoxelCoordinates((int)a.X,
								(int)a.Y,
								(int)a.Z);
	}

	/// <summary>
	/// Converts Global Chunk Coordinates to Global Voxel Coordinates
	/// </summary>
	/// <param name="a">The Global Chunk Coordinates to convert.</param>
	/// <returns>
	/// The Global Voxel Coordinates of the bottom Block of the North-West column of
	/// Blocks in the specified Chunk.
	/// </returns>
	public static explicit operator GlobalVoxelCoordinates(GlobalChunkCoordinates a)
	{
		return new GlobalVoxelCoordinates(WorldConstants.ChunkWidth * a.X, 0, WorldConstants.ChunkDepth * a.Z);
	}
	#endregion

	#region Constants

	/// <summary>
	/// A trio of 3D coordinates with components set to 0.0.
	/// </summary>
	public static readonly GlobalVoxelCoordinates Zero = new GlobalVoxelCoordinates(0, 0, 0);

	/// <summary>
	/// Coordinates with x = y = z = 1.
	/// </summary>
	public static readonly GlobalVoxelCoordinates One = new GlobalVoxelCoordinates(1, 1, 1);
	#endregion

	/// <summary>
	/// Determines whether this 3D coordinates and another are equal.
	/// </summary>
	/// <param name="other">The other coordinates.</param>
	/// <returns></returns>
	public bool Equals(GlobalVoxelCoordinates? other)
	{
		if (other is null)
			return false;
		else
			return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
	}

	/// <summary>
	/// Determines whether this and another object are equal.
	/// </summary>
	/// <param name="obj">The other object.</param>
	/// <returns></returns>
	public override bool Equals(object? obj)
	{
		return Equals(obj as GlobalVoxelCoordinates);
	}

	/// <summary>
	/// Returns the hash code for this 3D coordinates.
	/// </summary>
	/// <returns></returns>
	public override int GetHashCode()
	{
		unchecked
		{
			int result = X.GetHashCode();
			result = (result * 397) ^ Y.GetHashCode();
			result = (result * 397) ^ Z.GetHashCode();
			return result;
		}
	}
}