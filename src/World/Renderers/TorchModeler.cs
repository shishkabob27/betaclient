public static class TorchModeler
{
	public static void RenderBlock(Chunk chunk, int x, int y, int z, byte metadata, byte blocklight, byte skylight, ref List<Chunk.Vertex> vertices, ref List<uint> indices)
	{
		byte blockID = chunk.GetBlockID(x, y, z);
		BlockDefinition block = BlockRegistry.GetBlock(blockID);

		// Note: This would need proper texture atlas handling for Silk.NET
		int textureAtlasSize = 256; // Placeholder
		int textureAtlasBlockSize = 16; //16x16

		const float TorchOffset = 0.4375f;
		var color = Color.White.ToVector4();
		uint baseIndex = (uint)vertices.Count;
		
		//top face (positive y)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveY, metadata);

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 0.625f, z), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 0.625f, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 0.625f, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 0.625f, z + 1), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), System.Numerics.Vector3.UnitY, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 3);
			indices.Add(baseIndex + 2);
			
			baseIndex += 4;
		}

		// Front face (negative z)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeZ, metadata);

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z + TorchOffset), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 1, z + TorchOffset), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z + TorchOffset), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 1, z + TorchOffset), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitZ, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 3);
			indices.Add(baseIndex + 2);
			
			baseIndex += 4;
		}

		// Back face (Positive Z)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveZ, metadata);

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y, z + 1 - TorchOffset), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x, y + 1, z + 1 - TorchOffset), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y, z + 1 - TorchOffset), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1, y + 1, z + 1 - TorchOffset), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitZ, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			
			baseIndex += 4;
		}

		// Left face (Negative X)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeX, metadata);

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + TorchOffset, y, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + TorchOffset, y + 1, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + TorchOffset, y, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + TorchOffset, y + 1, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), -System.Numerics.Vector3.UnitX, color));

			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			indices.Add(baseIndex + 1);
			
			baseIndex += 4;
		}

		// Right face (Positive X)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveX, metadata);

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1 - TorchOffset, y, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1 - TorchOffset, y + 1, z), new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1 - TorchOffset, y, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x + 1 - TorchOffset, y + 1, z + 1), new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize), System.Numerics.Vector3.UnitX, color));

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