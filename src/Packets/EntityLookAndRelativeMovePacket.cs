using System.Numerics;

public struct EntityLookAndRelativeMovePacket : IPacket
{
	public byte ID { get { return 0x21; } }

	public int EntityID;
	public sbyte DeltaX, DeltaY, DeltaZ;
	public sbyte Yaw, Pitch;

	public void ReadPacket(MinecraftStream stream)
	{
		EntityID = stream.ReadInt32();
		DeltaX = stream.ReadInt8();
		DeltaY = stream.ReadInt8();
		DeltaZ = stream.ReadInt8();
		Yaw = stream.ReadInt8();
		Pitch = stream.ReadInt8();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt32(EntityID);
		stream.WriteInt8(DeltaX);
		stream.WriteInt8(DeltaY);
		stream.WriteInt8(DeltaZ);
		stream.WriteInt8(Yaw);
		stream.WriteInt8(Pitch);
	}

	public void Action()
	{
		//Logger.Debug("EntityLookAndRelativeMovePacket EntityID: " + EntityID + " DeltaX: " + DeltaX + " DeltaY: " + DeltaY + " DeltaZ: " + DeltaZ + " Yaw: " + Yaw + " Pitch: " + Pitch);

		//find the entity
		Entity entity = BetaClient.Game.World.GetEntity(EntityID);
		if (entity != null)
		{
			entity.Position += new Vector3((float)DeltaX / 32.0f, (float)DeltaY / 32.0f, (float)DeltaZ / 32.0f);
			entity.Yaw = Yaw;
			entity.Pitch = Pitch;
		}
		else
		{
			Logger.Warn("EntityLookAndRelativeMovePacket: Entity not found");
		}
	}
}