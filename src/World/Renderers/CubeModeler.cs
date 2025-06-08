public static class CubeModeler
{
	public static void RenderBlock(Chunk chunk, int x, int y, int z, byte metadata, byte blocklight, byte skylight, ref List<Chunk.Vertex> vertices, ref List<uint> indices)
	{
		byte blockID = chunk.GetBlockID(x, y, z);
		BlockDefinition block = BlockRegistry.GetBlock(blockID);

		// Texture atlas handling
		int textureAtlasSize = 256;
		int textureAtlasBlockSize = 16;

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

		uint baseIndex = (uint)vertices.Count;
		
		// Convert local chunk coordinates to world coordinates
		x = x + chunk.X * WorldConstants.ChunkWidth;
		z = z + chunk.Z * WorldConstants.ChunkDepth;
		
		if (!blockUp.Opaque && blockUp.ID != blockID) //top face (positive y)
		{
			// Get texture coordinates for top face
			Tuple<int, int> atlasCoords = GetTextureMap(blockID, BlockFace.PositiveY, metadata);
			float u1 = (float)atlasCoords.Item1 / textureAtlasSize;
			float v1 = (float)atlasCoords.Item2 / textureAtlasSize;
			float u2 = (float)(atlasCoords.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v2 = (float)(atlasCoords.Item2 + textureAtlasBlockSize) / textureAtlasSize;
			
			// Top face vertices
			var color = blockID == 2 ? new Color(158, 215, 109, 255).ToVector4() : Color.White.ToVector4();
			
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 1, z), new System.Numerics.Vector2(u1, v2), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 1, z + 1), new System.Numerics.Vector2(u1, v1), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 1, z), new System.Numerics.Vector2(u2, v2), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 1, z + 1), new System.Numerics.Vector2(u2, v1), System.Numerics.Vector3.UnitY, color));

			// Top face indices
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
			// Get texture coordinates for bottom face
			Tuple<int, int> atlasCoords = GetTextureMap(blockID, BlockFace.NegativeY, metadata);
			float u1 = (float)atlasCoords.Item1 / textureAtlasSize;
			float v1 = (float)atlasCoords.Item2 / textureAtlasSize;
			float u2 = (float)(atlasCoords.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v2 = (float)(atlasCoords.Item2 + textureAtlasBlockSize) / textureAtlasSize;
			
			var color = new Color(120, 120, 120, 255).ToVector4();
			
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z), new System.Numerics.Vector2(u1, v2), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z + 1), new System.Numerics.Vector2(u1, v1), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z), new System.Numerics.Vector2(u2, v2), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z + 1), new System.Numerics.Vector2(u2, v1), -System.Numerics.Vector3.UnitY, color));

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
			// Get texture coordinates for front face
			Tuple<int, int> atlasCoords = GetTextureMap(blockID, BlockFace.NegativeZ, metadata);
			float u1 = (float)atlasCoords.Item1 / textureAtlasSize;
			float v1 = (float)atlasCoords.Item2 / textureAtlasSize;
			float u2 = (float)(atlasCoords.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v2 = (float)(atlasCoords.Item2 + textureAtlasBlockSize) / textureAtlasSize;
			
			var color = Color.White.ToVector4();
			
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z), new System.Numerics.Vector2(u2, v2), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 1, z), new System.Numerics.Vector2(u2, v1), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z), new System.Numerics.Vector2(u1, v2), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 1, z), new System.Numerics.Vector2(u1, v1), -System.Numerics.Vector3.UnitZ, color));

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
			// Get texture coordinates for back face
			Tuple<int, int> atlasCoords = GetTextureMap(blockID, BlockFace.PositiveZ, metadata);
			float u1 = (float)atlasCoords.Item1 / textureAtlasSize;
			float v1 = (float)atlasCoords.Item2 / textureAtlasSize;
			float u2 = (float)(atlasCoords.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v2 = (float)(atlasCoords.Item2 + textureAtlasBlockSize) / textureAtlasSize;
			
			var color = Color.White.ToVector4();
			
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z + 1), new System.Numerics.Vector2(u1, v2), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 1, z + 1), new System.Numerics.Vector2(u1, v1), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z + 1), new System.Numerics.Vector2(u2, v2), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 1, z + 1), new System.Numerics.Vector2(u2, v1), System.Numerics.Vector3.UnitZ, color));

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
			// Get texture coordinates for left face
			Tuple<int, int> atlasCoords = GetTextureMap(blockID, BlockFace.NegativeX, metadata);
			float u1 = (float)atlasCoords.Item1 / textureAtlasSize;
			float v1 = (float)atlasCoords.Item2 / textureAtlasSize;
			float u2 = (float)(atlasCoords.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v2 = (float)(atlasCoords.Item2 + textureAtlasBlockSize) / textureAtlasSize;
			
			var color = new Color(155, 155, 155, 255).ToVector4();
			
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z), new System.Numerics.Vector2(u2, v2), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 1, z), new System.Numerics.Vector2(u2, v1), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z + 1), new System.Numerics.Vector2(u1, v2), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 1, z + 1), new System.Numerics.Vector2(u1, v1), -System.Numerics.Vector3.UnitX, color));

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
			// Get texture coordinates for right face
			Tuple<int, int> atlasCoords = GetTextureMap(blockID, BlockFace.PositiveX, metadata);
			float u1 = (float)atlasCoords.Item1 / textureAtlasSize;
			float v1 = (float)atlasCoords.Item2 / textureAtlasSize;
			float u2 = (float)(atlasCoords.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v2 = (float)(atlasCoords.Item2 + textureAtlasBlockSize) / textureAtlasSize;
			
			var color = new Color(155, 155, 155, 255).ToVector4();
			
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z), new System.Numerics.Vector2(u1, v2), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 1, z), new System.Numerics.Vector2(u1, v1), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z + 1), new System.Numerics.Vector2(u2, v2), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 1, z + 1), new System.Numerics.Vector2(u2, v1), System.Numerics.Vector3.UnitX, color));

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