public class CubeModeler : IBlockModeler
{
	public void RenderBlock(Chunk chunk, int x, int y, int z, ref List<System.Numerics.Vector3> vertices, ref List<System.Numerics.Vector2> texcoords, ref List<System.Numerics.Vector3> normals, ref List<Raylib_cs.Color> colors, ref List<ushort> indices)
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
		
		if (!blockUp.Opaque && blockUp.ID != blockID) //top face (positive y)
		{
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z + 1));


			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveY);
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
				Raylib_cs.Color grassColor = new Raylib_cs.Color(158, 184, 109, 255);
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

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeY);
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize));
			texcoords.Add(new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize));

			normals.Add(-System.Numerics.Vector3.UnitY);
			normals.Add(-System.Numerics.Vector3.UnitY);
			normals.Add(-System.Numerics.Vector3.UnitY);
			normals.Add(-System.Numerics.Vector3.UnitY);

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

		if (!blockFront.Opaque && blockFront.ID != blockID) // Front face (negative z)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeZ);
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

		if (!blockBack.Opaque && blockBack.ID != blockID) // Back face (Positive Z)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveZ);
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

		if (!blockLeft.Opaque && blockLeft.ID != blockID) // Left face (Negative X)
		{
			vertices.Add(new System.Numerics.Vector3(x, y, z));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x, y + 1, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.NegativeX);
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

		if (!blockRight.Opaque && blockRight.ID != blockID) // Right face (Positive X)
		{
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z));
			vertices.Add(new System.Numerics.Vector3(x + 1, y, z + 1));
			vertices.Add(new System.Numerics.Vector3(x + 1, y + 1, z + 1));

			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, BlockFace.PositiveX);
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

	public static Tuple<int, int> GetTextureMap(byte blockID, BlockFace face)
	{
		BlockDefinition block = BlockRegistry.GetBlock(blockID);
		Tuple<int, int> atlasCoordinates = new Tuple<int, int>(0, 14);
		if (block != null)
		{
			atlasCoordinates = block.GetTextureMap(0, face);
		}
		return new Tuple<int, int>(atlasCoordinates.Item1 * 16, atlasCoordinates.Item2 * 16);
	}
}