public class QuadModeler : IBlockModeler
{
	public void RenderBlock(Chunk chunk, int x, int y, int z, byte metadata, byte blocklight, byte skylight, ref List<System.Numerics.Vector3> vertices, ref List<System.Numerics.Vector2> texcoords, ref List<System.Numerics.Vector3> normals, ref List<Raylib_cs.Color> colors, ref List<ushort> indices)
	{
		byte blockID = chunk.GetBlockID(x, y, z);

		int textureAtlasSize = BetaClient.Instance.terrainAtlas.Width;
		int textureAtlasBlockSize = 16; //16x16

		Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, metadata);

		//if (chunk.GetBlockID(x, y, z - 1) == 0) // Front face (negative z)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z + 1));

			
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

		//if (chunk.GetBlockID(x, y, z + 1) == 0) // Back face (Positive Z)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z + 0));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z + 0));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z + 1));

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

		//if (chunk.GetBlockID(x - 1, y, z) == 0) // Left face (Negative X)
		{
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z + 1));

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

		//if (chunk.GetBlockID(x + 1, y, z) == 0) // Right face (Positive X)
		{
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x + 0, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 0, y + 1, z + 1));

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

	public static Tuple<int, int> GetTextureMap(byte blockID, byte metadata)
	{
		BlockDefinition block = BlockRegistry.GetBlock(blockID);
		Tuple<int, int> atlasCoordinates = new Tuple<int, int>(0, 14);
		if (block != null)
		{
			atlasCoordinates = block.GetTextureMap(metadata, 0);
		}
		return new Tuple<int, int>(atlasCoordinates.Item1 * 16, atlasCoordinates.Item2 * 16);
	}
}