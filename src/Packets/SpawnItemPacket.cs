using System.Diagnostics;

/// <summary>
/// Sent by servers to spawn item entities, I think.
/// </summary>
public struct SpawnItemPacket : IPacket
{
	public byte ID { get { return 0x15; } }

	public int EntityID;
	public short ItemID;
	public sbyte Count;
	public short Metadata;
	public int X, Y, Z;
	public sbyte Yaw;
	public sbyte Pitch;
	public sbyte Roll;

	public SpawnItemPacket(int entityID, short itemID, sbyte count, short metadata, int x, int y, int z, sbyte yaw, sbyte pitch, sbyte roll)
	{
		EntityID = entityID;
		ItemID = itemID;
		Count = count;
		Metadata = metadata;
		X = x;
		Y = y;
		Z = z;
		Yaw = yaw;
		Pitch = pitch;
		Roll = roll;
	}

	public void ReadPacket(MinecraftStream stream)
	{
		EntityID = stream.ReadInt32();
		ItemID = stream.ReadInt16();
		Count = stream.ReadInt8();
		Metadata = stream.ReadInt16();
		X = stream.ReadInt32();
		Y = stream.ReadInt32();
		Z = stream.ReadInt32();
		Yaw = stream.ReadInt8();
		Pitch = stream.ReadInt8();
		Roll = stream.ReadInt8();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt32(EntityID);
		stream.WriteInt16(ItemID);
		stream.WriteInt8(Count);
		stream.WriteInt16(Metadata);
		stream.WriteInt32(X);
		stream.WriteInt32(Y);
		stream.WriteInt32(Z);
		stream.WriteInt8(Yaw);
		stream.WriteInt8(Pitch);
		stream.WriteInt8(Roll);
	}

	public void Action()
	{
		Debug.WriteLine($"SpawnItem: {ItemID} x{Count} at {X}, {Y}, {Z}");
	}
}