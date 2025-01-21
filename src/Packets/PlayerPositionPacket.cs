/// <summary>
/// Sent by clients to inform the server of updates to their position.
/// </summary>
public struct PlayerPositionPacket : IPacket
{
	public byte ID { get { return 0x0B; } }

	public double X, Y, Z;
	/// <summary>
	/// The Y position of the player's eyes. This changes when crouching.
	/// </summary>
	public double Stance;
	public bool OnGround;

	public PlayerPositionPacket(double x, double y, double stance, double z, bool onGround)
	{
		X = x;
		Y = y;
		Stance = stance;
		Z = z;
		OnGround = onGround;
	}

	public void ReadPacket(MinecraftStream stream)
	{
		X = stream.ReadDouble();
		Y = stream.ReadDouble();
		Stance = stream.ReadDouble();
		Z = stream.ReadDouble();
		OnGround = stream.ReadBoolean();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteDouble(X);
		stream.WriteDouble(Y);
		stream.WriteDouble(Stance);
		stream.WriteDouble(Z);
		stream.WriteBoolean(OnGround);
	}

	public void Action()
	{
	}
}