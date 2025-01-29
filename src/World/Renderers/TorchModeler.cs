public class TorchModeler : IBlockModeler
{
	public void RenderBlock(Chunk chunk, int x, int y, int z, byte metadata, byte blocklight, byte skylight, ref List<System.Numerics.Vector3> vertices, ref List<System.Numerics.Vector2> texcoords, ref List<System.Numerics.Vector3> normals, ref List<Raylib_cs.Color> colors, ref List<ushort> indices)
	{
		byte blockID = chunk.GetBlockID(x, y, z);
		BlockDefinition block = BlockRegistry.GetBlock(blockID);

		int textureAtlasSize = BetaClient.Instance.terrainAtlas.Width;
		int textureAtlasBlockSize = 16; //16x16

		const float TorchOffset = 0.4375f;
		
		//top face (positive y)
		{
			vertices.Add(new System.Numerics.Vector3(x, y + 0.625f, z));
			vertices.Add(new System.Numerics.Vector3(x, y + 0.625f, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 0.625f, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 0.625f, z + 1));


			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveY, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));

			normals.Add(System.Numerics.Vector3.UnitY);
			normals.Add(System.Numerics.Vector3.UnitY);
			normals.Add(System.Numerics.Vector3.UnitY);
			normals.Add(System.Numerics.Vector3.UnitY);

			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);

			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 2));
		}

		// Front face (negative z)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z + TorchOffset));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z + TorchOffset));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + TorchOffset));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z + TorchOffset));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeZ, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));

			normals.Add(-System.Numerics.Vector3.UnitZ);
			normals.Add(-System.Numerics.Vector3.UnitZ);
			normals.Add(-System.Numerics.Vector3.UnitZ);
			normals.Add(-System.Numerics.Vector3.UnitZ);

			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);

			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 2));
		}

		// Back face (Positive Z)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z + 1 - TorchOffset));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z + 1 - TorchOffset));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + 1 - TorchOffset));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z + 1 - TorchOffset));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveZ, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));

			normals.Add(System.Numerics.Vector3.UnitZ);
			normals.Add(System.Numerics.Vector3.UnitZ);
			normals.Add(System.Numerics.Vector3.UnitZ);
			normals.Add(System.Numerics.Vector3.UnitZ);

			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);

			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 1));
		}

		// Left face (Negative X)
		{
			vertices.Add(new System.Numerics.Vector3(x + TorchOffset, y, z));
			vertices.Add(new System.Numerics.Vector3(x + TorchOffset, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x + TorchOffset, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + TorchOffset, y + 1, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeX, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));

			normals.Add(-System.Numerics.Vector3.UnitX);
			normals.Add(-System.Numerics.Vector3.UnitX);
			normals.Add(-System.Numerics.Vector3.UnitX);
			normals.Add(-System.Numerics.Vector3.UnitX);

			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);

			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 3));
		}

		// Right face (Positive X)
		{
			vertices.Add(new System.Numerics.Vector3(x + 1 - TorchOffset, y, z));
			vertices.Add(new System.Numerics.Vector3(x + 1 - TorchOffset, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x + 1 - TorchOffset, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1 - TorchOffset, y + 1, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveX, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));

			normals.Add(System.Numerics.Vector3.UnitX);
			normals.Add(System.Numerics.Vector3.UnitX);
			normals.Add(System.Numerics.Vector3.UnitX);
			normals.Add(System.Numerics.Vector3.UnitX);

			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);
			colors.Add(Raylib_cs.Color.White);

			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 2));
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