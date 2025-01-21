public struct DestroyEntityPacket : IPacket
{
	public byte ID { get { return 0x1D; } }

	public int EntityID;

	public DestroyEntityPacket(int entityID)
	{
		EntityID = entityID;
	}

	public void ReadPacket(MinecraftStream stream)
	{
		EntityID = stream.ReadInt32();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt32(EntityID);
	}

	public void Action()
	{
		World world = BetaClient.Game.World;
		Entity entity = world.GetEntity(EntityID);
		if (entity != null)
		{
			world.Entities.Remove(EntityID);
			Logger.Info($"Entity {EntityID} destroyed");
		}
	}
}