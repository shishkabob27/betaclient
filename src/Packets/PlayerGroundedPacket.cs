using System.Diagnostics;

/// <summary>
/// Sent by clients to update whether or not the player is on the ground.
/// Probably best to just ignore this.
/// </summary>
public struct PlayerGroundedPacket : IPacket
{
    public byte ID { get { return 0x0A; }}

    public bool OnGround;

    public PlayerGroundedPacket(bool onGround)
    {
        OnGround = onGround;
    }

    public void ReadPacket(MinecraftStream stream)
    {
        OnGround = stream.ReadBoolean();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteBoolean(OnGround);
    }

	public void Action()
	{
	}
}