public class World
{
	public Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();
	public int PlayerID { get; set; } = -1;

	public List<ChunkPreamblePacket> IncomingPreChunks = new List<ChunkPreamblePacket>();
	public List<ChunkDataPacket> IncomingChunks = new List<ChunkDataPacket>();
	public List<Chunk> Chunks = new List<Chunk>();

	public Vector3 SpawnPosition { get; set; }
	public long Time { get; set; }

	public sbyte Dimension { get; set; } = 0;

	public bool ReceivedFirstPlayerPosition { get; set; } = false;

	public World()
	{
	}

	public void AddEntity(Entity entity)
	{
		Entities.Add(entity.EntityID, entity);
	}

	public Entity GetEntity(int entityID)
	{
		if (Entities.ContainsKey(entityID))
		{
			return Entities[entityID];
		}
		return null;
	}

	public PlayerEntity GetPlayer()
	{
		return (PlayerEntity)GetEntity(PlayerID);
	}

	public void AddChunk(Chunk chunk)
	{
		Chunks.Add(chunk);
	}

	public void RemoveChunk(Chunk chunk)
	{
		Chunks.Remove(chunk);
	}

	public Chunk GetChunk(int x, int z)
	{
		foreach (Chunk chunk in Chunks)
		{
			if (chunk.X == x && chunk.Z == z)
			{
				return chunk;
			}
		}
		return null;
	}

	public byte GetBlockID(int x, int y, int z)
	{
		Chunk chunk = GetChunk(x / WorldConstants.ChunkWidth, z / WorldConstants.ChunkDepth);
		if (chunk != null)
		{
			return chunk.GetBlockID(x % WorldConstants.ChunkWidth, y, z % WorldConstants.ChunkDepth);
		}
		return 0;
	}

	public void DestroyNonPlayerEntities()
	{
		List<int> toRemove = new List<int>();
		foreach (var entity in Entities)
		{
			if (entity.Value is PlayerEntity player && player.EntityID != PlayerID)
			{
				toRemove.Add(entity.Key);
			}
		}
		foreach (var id in toRemove)
		{
			Entities.Remove(id);
		}
	}
}