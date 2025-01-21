/// <summary>
/// Disconnects from a server or kicks a player. This is the last packet sent.
/// </summary>
public struct DisconnectPacket : IPacket
{
	public byte ID { get { return 0xFF; } }

	public DisconnectPacket(string reason)
	{
		Reason = reason;
	}

	public string Reason;

	public void ReadPacket(MinecraftStream stream)
	{
		Reason = stream.ReadString();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteString(Reason);
	}

	public void Action()
	{
		Logger.Debug("DisconnectPacket Reason: " + Reason);
	}
}