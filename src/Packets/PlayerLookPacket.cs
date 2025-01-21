/// <summary>
/// Sent to update the rotation of the player's head and body.
/// </summary>
public struct PlayerLookPacket : IPacket
{
    public byte ID { get { return 0x0C; } }

    public float Yaw, Pitch;
    public bool OnGround;

	public PlayerLookPacket(float yaw, float pitch, bool onGround)
	{
		Yaw = yaw;
		Pitch = pitch;
		OnGround = onGround;
	}

    public void ReadPacket(MinecraftStream stream)
    {
        Yaw = stream.ReadSingle();
        Pitch = stream.ReadSingle();
        OnGround = stream.ReadBoolean();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteSingle(Yaw);
        stream.WriteSingle(Pitch);
        stream.WriteBoolean(OnGround);
    }

	public void Action()
	{
		// Do nothing
	}
}