/// <summary>
/// Sent by clients when the player clicks "Respawn" after death. Sent by servers to confirm
/// the respawn, and to respawn players in different dimensions (i.e. when using a portal).
/// </summary>
public struct RespawnPacket : IPacket
{
	public byte ID { get { return 0x09; } }

	public sbyte Dimension;

	public void ReadPacket(MinecraftStream stream)
	{
		Dimension = stream.ReadInt8();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt8(Dimension);
	}

	public void Action()
	{
		BetaClient.Game.World.Dimension = Dimension;
	}
}