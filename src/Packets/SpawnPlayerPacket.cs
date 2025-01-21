public struct SpawnPlayerPacket : IPacket
{
    public byte ID { get { return 0x14; } }

    public int EntityID;
    public string PlayerName;
    public int X, Y, Z;
    public sbyte Yaw, Pitch;
    /// <summary>
    /// Note that this should be 0 for "no item".
    /// </summary>
    public short CurrentItem;

    public SpawnPlayerPacket(int entityID, string playerName, int x, int y, int z, sbyte yaw, sbyte pitch, short currentItem)
    {
        EntityID = entityID;
        PlayerName = playerName;
        X = x;
        Y = y;
        Z = z;
        Yaw = yaw;
        Pitch = pitch;
        CurrentItem = currentItem;
    }

    public void ReadPacket(MinecraftStream stream)
    {
        EntityID = stream.ReadInt32();
        PlayerName = stream.ReadString();
        X = stream.ReadInt32();
        Y = stream.ReadInt32();
        Z = stream.ReadInt32();
        Yaw = stream.ReadInt8();
        Pitch = stream.ReadInt8();
        CurrentItem = stream.ReadInt16();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(EntityID);
        stream.WriteString(PlayerName);
        stream.WriteInt32(X);
        stream.WriteInt32(Y);
        stream.WriteInt32(Z);
        stream.WriteInt8(Yaw);
        stream.WriteInt8(Pitch);
        stream.WriteInt16(CurrentItem);
    }

	public void Action()
	{
		Logger.Debug($"SpawnPlayer: {PlayerName} ({EntityID}) at {X}, {Y}, {Z} yaw {Yaw} pitch {Pitch} with item {CurrentItem}");

        PlayerEntity player = new PlayerEntity()
        {
            EntityID = EntityID,
            Username = PlayerName,
            Position = new Vector3(X / 32.0, Y / 32.0, Z / 32.0),
            Yaw = Yaw,
            Pitch = Pitch
        };

        BetaClient.Game.World.AddEntity(player);
	}
	
}