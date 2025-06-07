using System.Numerics;

/// <summary>
/// Used to teleport entities to arbitrary locations.
/// </summary>
public struct EntityTeleportPacket : IPacket
{
	public byte ID { get { return 0x22; } }

	public int EntityID;
	public int X, Y, Z;
	public sbyte Yaw, Pitch;

	public EntityTeleportPacket(int entityID, int x, int y, int z, sbyte yaw, sbyte pitch)
	{
		EntityID = entityID;
		X = x;
		Y = y;
		Z = z;
		Yaw = yaw;
		Pitch = pitch;
	}

	public void ReadPacket(MinecraftStream stream)
	{
		EntityID = stream.ReadInt32();
		X = stream.ReadInt32();
		Y = stream.ReadInt32();
		Z = stream.ReadInt32();
		Yaw = stream.ReadInt8();
		Pitch = stream.ReadInt8();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt32(EntityID);
		stream.WriteInt32(X);
		stream.WriteInt32(Y);
		stream.WriteInt32(Z);
		stream.WriteInt8(Yaw);
		stream.WriteInt8(Pitch);
	}

	public void Action()
	{
		Logger.Debug("EntityTeleportPacket EntityID: " + EntityID + " X: " + X + " Y: " + Y + " Z: " + Z + " Yaw: " + Yaw + " Pitch: " + Pitch);

		// Find the entity
		Entity entity = BetaClient.Game.World.GetEntity(EntityID);
		if (entity == null)
		{
			Logger.Debug("EntityTeleportPacket: Entity not found");
			return;
		}

		// Teleport the entity
		entity.Position = new Vector3(X / 32f, Y / 32f, Z / 32f);
		entity.Yaw = Yaw;
		entity.Pitch = Pitch;
	}
}