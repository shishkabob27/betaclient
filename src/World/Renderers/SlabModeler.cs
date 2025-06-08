public static class SlabModeler
{
	public static void RenderBlock(Chunk chunk, int x, int y, int z, byte metadata, byte blocklight, byte skylight, ref List<Chunk.Vertex> vertices, ref List<uint> indices)
	{
		byte blockID = chunk.GetBlockID(x, y, z);
		BlockDefinition block = BlockRegistry.GetBlock(blockID);

		// Note: This would need proper texture atlas handling for Silk.NET
		int textureAtlasSize = 256; // Placeholder
		int textureAtlasBlockSize = 16; //16x16

		uint baseIndex = (uint)vertices.Count;

		//neighboring blocks
		byte blockIDUp = chunk.GetBlockID(x, y + 1, z);
		byte blockIDDown = chunk.GetBlockID(x, y - 1, z);
		byte blockIDFront = chunk.GetBlockID(x, y, z - 1);
		byte blockIDBack = chunk.GetBlockID(x, y, z + 1);
		byte blockIDLeft = chunk.GetBlockID(x - 1, y, z);
		byte blockIDRight = chunk.GetBlockID(x + 1, y, z);

		BlockDefinition blockUp = BlockRegistry.GetBlock(blockIDUp);
		BlockDefinition blockDown = BlockRegistry.GetBlock(blockIDDown);
		BlockDefinition blockFront = BlockRegistry.GetBlock(blockIDFront);
		BlockDefinition blockBack = BlockRegistry.GetBlock(blockIDBack);
		BlockDefinition blockLeft = BlockRegistry.GetBlock(blockIDLeft);
		BlockDefinition blockRight = BlockRegistry.GetBlock(blockIDRight);
		
		if (true) //top face (positive y)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveY, metadata);
			
			//temp grass color
			var color = blockID == 2 ? new Color(158, 215, 109, 255).ToVector4() : Color.White.ToVector4();

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + .5f, z), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + .5f, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + .5f, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + .5f, z + 1), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), System.Numerics.Vector3.UnitY, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 3);
			indices.Add(baseIndex + 2);
			
			baseIndex += 4;
		}
		
		if (!blockDown.Opaque && blockDown.ID != blockID) //bottom face (negative y)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeY, metadata);
			var color = new Color(120, 120, 120, 255).ToVector4();

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z + 1), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), -System.Numerics.Vector3.UnitY, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			
			baseIndex += 4;
		}

		if (!blockFront.Opaque && blockFront.ID != blockID) // Front face (negative z)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeZ, metadata);
			var color = Color.White.ToVector4();

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + .5f, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + .5f, z), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitZ, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 3);
			indices.Add(baseIndex + 2);
			
			baseIndex += 4;
		}

		if (!blockBack.Opaque && blockBack.ID != blockID) // Back face (Positive Z)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveZ, metadata);
			var color = Color.White.ToVector4();

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z + 1), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + .5f, z + 1), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + .5f, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitZ, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			
			baseIndex += 4;
		}

		if (!blockLeft.Opaque && blockLeft.ID != blockID) // Left face (Negative X)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeX, metadata);
			var color = new Color(155, 155, 155, 255).ToVector4();

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + .5f, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + .5f, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitX, color));

			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			indices.Add(baseIndex + 1);
			
			baseIndex += 4;
		}

		if (!blockRight.Opaque && blockRight.ID != blockID) // Right face (Positive X)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveX, metadata);
			var color = new Color(155, 155, 155, 255).ToVector4();

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + .5f, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, ((float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + .5f, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2  / textureAtlasSize), System.Numerics.Vector3.UnitX, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 3);
			indices.Add(baseIndex + 2);
		}
	}

	public static Tuple<int, int> GetTextureMap(byte blockID, BlockFace face, byte metadata)
	{
		BlockDefinition block = BlockRegistry.GetBlock(blockID);
		Tuple<int, int> atlasCoordinates = new Tuple<int, int>(0, 14);
		if (block != null)
		{
			atlasCoordinates = block.GetTextureMap(metadata, face);
		}
		return new Tuple<int, int>(atlasCoordinates.Item1 * 16, atlasCoordinates.Item2 * 16);
	}
}