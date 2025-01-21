/// <summary>
/// Sent by the server to specify the coordinates of the spawn point. This only affects what the
/// compass item points to.
/// </summary>
public struct SpawnPositionPacket : IPacket
{
	public byte ID { get { return 0x06; } }

	public SpawnPositionPacket(int x, int y, int z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public int X, Y, Z;

	public void ReadPacket(MinecraftStream stream)
	{
		X = stream.ReadInt32();
		Y = stream.ReadInt32();
		Z = stream.ReadInt32();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt32(X);
		stream.WriteInt32(Y);
		stream.WriteInt32(Z);
	}

	public void Action()
	{
		Logger.Debug($"Spawn position: {X}, {Y}, {Z}");

		BetaClient.Game.World.SpawnPosition = new Vector3(X, Y, Z);
	}
}