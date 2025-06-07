public class QuadModeler : IBlockModeler
{
	public void RenderBlock(Chunk chunk, int x, int y, int z, byte metadata, byte blocklight, byte skylight, ref List<Chunk.Vertex> vertices, ref List<uint> indices)
	{
		byte blockID = chunk.GetBlockID(x, y, z);

		// Note: This would need proper texture atlas handling for Silk.NET
		// For now, using placeholder texture coordinates
		int textureAtlasSize = 256; // Placeholder
		int textureAtlasBlockSize = 16; //16x16

		var color = Color.White.ToVector4();
		uint baseIndex = (uint)vertices.Count;

		// Use consistent local coordinates for all vertices
		float x0 = x;
		float x1 = x + 1;
		float y0 = y;
		float y1 = y + 1;
		float z0 = z;
		float z1 = z + 1;

		// Front face (negative z)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, metadata, BlockFace.NegativeZ);
			float u0 = (float)atlasCoordinates.Item1 / textureAtlasSize;
			float u1 = (float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v0 = (float)atlasCoordinates.Item2 / textureAtlasSize;
			float v1 = (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize;

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y0, z0), new System.Numerics.Vector2(u0, v1), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y0, z0), new System.Numerics.Vector2(u1, v1), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y1, z0), new System.Numerics.Vector2(u1, v0), -System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y1, z0), new System.Numerics.Vector2(u0, v0), -System.Numerics.Vector3.UnitZ, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			
			baseIndex += 4;
		}

		// Back face (positive z)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, metadata, BlockFace.PositiveZ);
			float u0 = (float)atlasCoordinates.Item1 / textureAtlasSize;
			float u1 = (float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v0 = (float)atlasCoordinates.Item2 / textureAtlasSize;
			float v1 = (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize;

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y0, z1), new System.Numerics.Vector2(u0, v1), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y0, z1), new System.Numerics.Vector2(u1, v1), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y1, z1), new System.Numerics.Vector2(u1, v0), System.Numerics.Vector3.UnitZ, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y1, z1), new System.Numerics.Vector2(u0, v0), System.Numerics.Vector3.UnitZ, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			
			baseIndex += 4;
		}

		// Left face (negative x)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, metadata, BlockFace.NegativeX);
			float u0 = (float)atlasCoordinates.Item1 / textureAtlasSize;
			float u1 = (float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v0 = (float)atlasCoordinates.Item2 / textureAtlasSize;
			float v1 = (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize;

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y0, z1), new System.Numerics.Vector2(u0, v1), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y0, z0), new System.Numerics.Vector2(u1, v1), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y1, z0), new System.Numerics.Vector2(u1, v0), -System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y1, z1), new System.Numerics.Vector2(u0, v0), -System.Numerics.Vector3.UnitX, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			
			baseIndex += 4;
		}

		// Right face (positive x)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, metadata, BlockFace.PositiveX);
			float u0 = (float)atlasCoordinates.Item1 / textureAtlasSize;
			float u1 = (float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v0 = (float)atlasCoordinates.Item2 / textureAtlasSize;
			float v1 = (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize;

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y0, z0), new System.Numerics.Vector2(u0, v1), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y0, z1), new System.Numerics.Vector2(u1, v1), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y1, z1), new System.Numerics.Vector2(u1, v0), System.Numerics.Vector3.UnitX, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y1, z0), new System.Numerics.Vector2(u0, v0), System.Numerics.Vector3.UnitX, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			
			baseIndex += 4;
		}

		// Top face (positive y)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, metadata, BlockFace.PositiveY);
			float u0 = (float)atlasCoordinates.Item1 / textureAtlasSize;
			float u1 = (float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v0 = (float)atlasCoordinates.Item2 / textureAtlasSize;
			float v1 = (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize;

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y1, z0), new System.Numerics.Vector2(u0, v1), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y1, z0), new System.Numerics.Vector2(u1, v1), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y1, z1), new System.Numerics.Vector2(u1, v0), System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y1, z1), new System.Numerics.Vector2(u0, v0), System.Numerics.Vector3.UnitY, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
			
			baseIndex += 4;
		}

		// Bottom face (negative y)
		{
			Tuple<int, int> atlasCoordinates = GetTextureMap(blockID, metadata, BlockFace.NegativeY);
			float u0 = (float)atlasCoordinates.Item1 / textureAtlasSize;
			float u1 = (float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize;
			float v0 = (float)atlasCoordinates.Item2 / textureAtlasSize;
			float v1 = (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize;

			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y0, z1), new System.Numerics.Vector2(u0, v1), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y0, z1), new System.Numerics.Vector2(u1, v1), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x1, y0, z0), new System.Numerics.Vector2(u1, v0), -System.Numerics.Vector3.UnitY, color));
			vertices.Add(new Chunk.Vertex(new System.Numerics.Vector3(x0, y0, z0), new System.Numerics.Vector2(u0, v0), -System.Numerics.Vector3.UnitY, color));

			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 1);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 0);
			indices.Add(baseIndex + 2);
			indices.Add(baseIndex + 3);
		}
	}

	public static Tuple<int, int> GetTextureMap(byte blockID, byte metadata, BlockFace face)
	{
		BlockDefinition block = BlockRegistry.GetBlock(blockID);
		Tuple<int, int> atlasCoordinates = new Tuple<int, int>(0, 14);
		if (block != null)
		{
			atlasCoordinates = block.GetTextureMap(metadata, face);
		}
		
		// Debug logging
		Console.WriteLine($"QuadModeler - BlockID: {blockID}, Face: {face}, Atlas: ({atlasCoordinates.Item1}, {atlasCoordinates.Item2}), Final: ({atlasCoordinates.Item1 * 16}, {atlasCoordinates.Item2 * 16})");
		
		return new Tuple<int, int>(atlasCoordinates.Item1 * 16, atlasCoordinates.Item2 * 16);
	}
}