using Raylib_cs;

public class SlabModeler : IBlockModeler
{
	public void RenderBlock(Chunk chunk, int x, int y, int z, byte metadata, byte blocklight, byte skylight, ref List<System.Numerics.Vector3> vertices, ref List<System.Numerics.Vector2> texcoords, ref List<System.Numerics.Vector3> normals, ref List<Raylib_cs.Color> colors, ref List<ushort> indices)
	{
		byte blockID = chunk.GetBlockID(x, y, z);
		BlockDefinition block = BlockRegistry.GetBlock(blockID);

		int textureAtlasSize = BetaClient.Instance.terrainAtlas.Width;
		int textureAtlasBlockSize = 16; //16x16

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
			vertices.Add(new System.Numerics.Vector3(x, y + .5f, z));
			vertices.Add(new System.Numerics.Vector3(x, y + .5f, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + .5f, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + .5f, z + 1));


			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveY, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));

			normals.Add(System.Numerics.Vector3.UnitY);
			normals.Add(System.Numerics.Vector3.UnitY);
			normals.Add(System.Numerics.Vector3.UnitY);
			normals.Add(System.Numerics.Vector3.UnitY);

			//temp grass color
			if (blockID == 2)
			{
				Raylib_cs.Color grassColor = new Raylib_cs.Color(158, 215, 109, 255);
				colors.Add(grassColor);
				colors.Add(grassColor);
				colors.Add(grassColor);
				colors.Add(grassColor);
			}
			else
			{
				colors.Add(Raylib_cs.Color.White);
				colors.Add(Raylib_cs.Color.White);
				colors.Add(Raylib_cs.Color.White);
				colors.Add(Raylib_cs.Color.White);
			}

			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 2));
		}
		
		if (!blockDown.Opaque && blockDown.ID != blockID) //bottom face (negative y)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z));
			vertices.Add(new System.Numerics.Vector3(x, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeY, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));

			normals.Add(-System.Numerics.Vector3.UnitY);
			normals.Add(-System.Numerics.Vector3.UnitY);
			normals.Add(-System.Numerics.Vector3.UnitY);
			normals.Add(-System.Numerics.Vector3.UnitY);

			for (int i = 0; i < 4; i++)
			{
				colors.Add(new Color(120, 120, 120, 255));
			}

			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 1));
		}

		if (!blockFront.Opaque && blockFront.ID != blockID) // Front face (negative z)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z));
			vertices.Add(new System.Numerics.Vector3(x, y + .5f, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + .5f, z));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeZ, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));

			normals.Add(-System.Numerics.Vector3.UnitZ);
			normals.Add(-System.Numerics.Vector3.UnitZ);
			normals.Add(-System.Numerics.Vector3.UnitZ);
			normals.Add(-System.Numerics.Vector3.UnitZ);

			for (int i = 0; i < 4; i++)
			{
				colors.Add(new Color(255, 255, 255, 255));
			}

			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 2));
		}

		if (!blockBack.Opaque && blockBack.ID != blockID) // Back face (Positive Z)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x, y + .5f, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + .5f, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveZ, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));

			normals.Add(System.Numerics.Vector3.UnitZ);
			normals.Add(System.Numerics.Vector3.UnitZ);
			normals.Add(System.Numerics.Vector3.UnitZ);
			normals.Add(System.Numerics.Vector3.UnitZ);

			for (int i = 0; i < 4; i++)
			{
				colors.Add(new Color(255, 255, 255, 255));
			}

			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 1));
		}

		if (!blockLeft.Opaque && blockLeft.ID != blockID) // Left face (Negative X)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z));
			vertices.Add(new System.Numerics.Vector3(x, y + .5f, z));
			vertices.Add(new System.Numerics.Vector3(x, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x, y + .5f, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeX, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));

			normals.Add(-System.Numerics.Vector3.UnitX);
			normals.Add(-System.Numerics.Vector3.UnitX);
			normals.Add(-System.Numerics.Vector3.UnitX);
			normals.Add(-System.Numerics.Vector3.UnitX);

			for (int i = 0; i < 4; i++)
			{
				colors.Add(new Color(155, 155, 155, 255));
			}

			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 3));
			indices.Add((ushort)(vertices.Count - 4));
			indices.Add((ushort)(vertices.Count - 2));
			indices.Add((ushort)(vertices.Count - 1));
			indices.Add((ushort)(vertices.Count - 3));
		}

		if (!blockRight.Opaque && blockRight.ID != blockID) // Right face (Positive X)
		{
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + .5f, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + .5f, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveX, metadata);
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)((atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, ((float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / 2) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2  / textureAtlasSize));

			normals.Add(System.Numerics.Vector3.UnitX);
			normals.Add(System.Numerics.Vector3.UnitX);
			normals.Add(System.Numerics.Vector3.UnitX);
			normals.Add(System.Numerics.Vector3.UnitX);

			for (int i = 0; i < 4; i++)
			{
				colors.Add(new Color(155, 155, 155, 255));
			}

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