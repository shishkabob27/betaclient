public struct AnimationPacket : IPacket
{
    public enum PlayerAnimation
    {
        None = 0,
        SwingArm = 1,
        TakeDamage = 2,
        LeaveBed = 3,
        Unknown = 102,
        Crouch = 104,
        Uncrouch = 105,
    }

    public byte ID { get { return 0x12; } }

    public int EntityID;
    public PlayerAnimation Animation;

    public AnimationPacket(int entityID, PlayerAnimation animation)
    {
        EntityID = entityID;
        Animation = animation;
    }

    public void ReadPacket(MinecraftStream stream)
    {
        EntityID = stream.ReadInt32();
        Animation = (PlayerAnimation)stream.ReadInt8();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(EntityID);
        stream.WriteInt8((sbyte)Animation);
    }

	public void Action()
	{
		Logger.Debug($"Animation: {EntityID} {Animation}");
	}
}