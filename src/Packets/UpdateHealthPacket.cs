/// <summary>
/// Sent by servers to inform clients of their current health.
/// </summary>
public struct UpdateHealthPacket : IPacket
{
    public byte ID { get { return 0x08; } }

    public UpdateHealthPacket(short health)
    {
        Health = health;
    }

    public short Health;

    public void ReadPacket(MinecraftStream stream)
    {
        Health = stream.ReadInt16();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt16(Health);
    }

	public void Action()
	{
		// Update the player's health
		PlayerEntity player = BetaClient.Game.World.GetPlayer();
		player.Health = Health;
	}
}