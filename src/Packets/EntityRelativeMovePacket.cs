using System.Numerics;

public struct EntityRelativeMovePacket : IPacket
{
	public byte ID { get { return 0x1F; } }

	public int EntityID;
	public sbyte DeltaX, DeltaY, DeltaZ;

	public void ReadPacket(MinecraftStream stream)
	{
		EntityID = stream.ReadInt32();
		DeltaX = stream.ReadInt8();
		DeltaY = stream.ReadInt8();
		DeltaZ = stream.ReadInt8();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt32(EntityID);
		stream.WriteInt8(DeltaX);
		stream.WriteInt8(DeltaY);
		stream.WriteInt8(DeltaZ);
	}

	public void Action()
	{
		//Logger.Debug("EntityRelativeMovePacket EntityID: " + EntityID + " DeltaX: " + DeltaX + " DeltaY: " + DeltaY + " DeltaZ: " + DeltaZ);

		//find the entity
		Entity entity = BetaClient.Game.World.GetEntity(EntityID);
		if (entity != null)
		{
			entity.Position += new Vector3((float)DeltaX / 32.0f, (float)DeltaY / 32.0f, (float)DeltaZ / 32.0f);
		}
		else
		{
			Logger.Warn("EntityRelativeMovePacket: Entity not found");
		}
	}
}