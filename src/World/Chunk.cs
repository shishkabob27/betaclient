using System.Diagnostics;
using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
public class Chunk
{
	public int X, Z;
	public bool HasRecivedData = false;
	public byte[] _blockIDs;

	public Model[] BlockFaceModel = new Model[6];

	public List<BoundingBox> BoundingBoxes = new List<BoundingBox>();

	public Chunk(int x, int z)
	{
		X = x;
		Z = z;
	}

	public void UpdateChunkData(ChunkDataPacket packet)
	{
		MemoryStream compressedStream = new MemoryStream(packet.CompressedData);

		using (var ds = new ZLibStream(compressedStream, CompressionMode.Decompress))
        {
			HasRecivedData = true;
	
			int blockCount = WorldConstants.ChunkDepth * WorldConstants.ChunkWidth * WorldConstants.Height * 5 / 2;
			_blockIDs = new byte[blockCount];
			
			ds.Read(_blockIDs, 0, blockCount);

			RegenerateMesh(BlockFace.NegativeY);
			RegenerateMesh(BlockFace.PositiveY);
			RegenerateMesh(BlockFace.NegativeZ);
			RegenerateMesh(BlockFace.PositiveZ);
			RegenerateMesh(BlockFace.NegativeX);
			RegenerateMesh(BlockFace.PositiveX);
			RegenerateBoundingBoxes();
        }
	}

	public void RegenerateAllMesh()
	{
		RegenerateMesh(BlockFace.NegativeY);
		RegenerateMesh(BlockFace.PositiveY);
		RegenerateMesh(BlockFace.NegativeZ);
		RegenerateMesh(BlockFace.PositiveZ);
		RegenerateMesh(BlockFace.NegativeX);
		RegenerateMesh(BlockFace.PositiveX);
	}

	public unsafe void RegenerateMesh(BlockFace face)
	{
		if (!HasRecivedData) return;

		int vertexCount = 0, triangleCount = 0;

		// Calculate vertex count and triangle count
		for (int x = 0; x < WorldConstants.ChunkWidth; x++)
		{
			for (int y = 0; y < WorldConstants.Height; y++)
			{
				for (int z = 0; z < WorldConstants.ChunkDepth; z++)
				{
					byte blockID = GetBlockID(x, y, z);
					if (blockID == 0) continue;

					// Check if we need to add a face
					switch (face)
					{
						case BlockFace.NegativeY:
							if (GetBlockID(x, y - 1, z) == 0) triangleCount += 2; // Bottom face
							break;
						case BlockFace.PositiveY:
							if (GetBlockID(x, y + 1, z) == 0) triangleCount += 2; // Top face
							break;
						case BlockFace.NegativeZ:
							if (GetBlockID(x, y, z - 1) == 0) triangleCount += 2; // Front face
							break;
						case BlockFace.PositiveZ:
							if (GetBlockID(x, y, z + 1) == 0) triangleCount += 2; // Back face
							break;
						case BlockFace.NegativeX:
							if (GetBlockID(x - 1, y, z) == 0) triangleCount += 2; // Left face
							break;
						case BlockFace.PositiveX:	
							if (GetBlockID(x + 1, y, z) == 0) triangleCount += 2; // Right face
							break;
					}			
				}
			}
		}

		vertexCount = triangleCount * 3;

		// Create and allocate the mesh
		Mesh mesh = new(vertexCount, triangleCount);
		mesh.AllocVertices();
		mesh.AllocTexCoords();
		mesh.AllocNormals();
		mesh.AllocIndices();
		Span<System.Numerics.Vector3> vertices = mesh.VerticesAs<System.Numerics.Vector3>();
		Span<System.Numerics.Vector2> texcoords = mesh.TexCoordsAs<System.Numerics.Vector2>();
		Span<System.Numerics.Vector3> normals = mesh.NormalsAs<System.Numerics.Vector3>();
		Span<Color> colors = mesh.ColorsAs<Color>();
		Span<ushort> indices = mesh.IndicesAs<ushort>();

		int vertexIndex = 0;
		int indexOffset = 0;

		//texture atlas
		int textureAtlasSize = BetaClient.Instance.terrainAtlas.Width;
		int textureAtlasBlockSize = 16; //16x16

		for (int x = 0; x < WorldConstants.ChunkWidth; x++)
		{
			for (int y = 0; y < WorldConstants.Height; y++)
			{
				for (int z = 0; z < WorldConstants.ChunkDepth; z++)
				{
					byte blockID = GetBlockID(x, y, z);
					if (blockID == 0) continue;

					//get altas coordinates
					Tuple<int,int> atlasCoordinates = new Tuple<int, int>(0, 14);
					BlockDefinition block = BlockRegistry.GetBlock(blockID);
					if (block != null)
					{
						atlasCoordinates = BlockRegistry.GetBlock(blockID).GetTextureMap((byte)0, face);
					}

					//convert to pixel coordinates
					atlasCoordinates = new Tuple<int, int>(atlasCoordinates.Item1 * textureAtlasBlockSize, atlasCoordinates.Item2 * textureAtlasBlockSize);

					
					// Check if we need to add a face (currently only top face)
					switch (face)
					{
						case BlockFace.NegativeY:
							if (GetBlockID(x, y - 1, z) == 0)
							{
								// Bottom face vertices
								vertices[vertexIndex] = new System.Numerics.Vector3(x, y, z);
								vertices[vertexIndex + 1] = new System.Numerics.Vector3(x, y, z + 1);
								vertices[vertexIndex + 2] = new System.Numerics.Vector3(x + 1, y, z);
								vertices[vertexIndex + 3] = new System.Numerics.Vector3(x + 1, y, z + 1);

								texcoords[vertexIndex] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);
								texcoords[vertexIndex + 1] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 2] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);
								texcoords[vertexIndex + 3] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);

								normals[vertexIndex] = -System.Numerics.Vector3.UnitY;
								normals[vertexIndex + 1] = -System.Numerics.Vector3.UnitY;
								normals[vertexIndex + 2] = -System.Numerics.Vector3.UnitY;
								normals[vertexIndex + 3] = -System.Numerics.Vector3.UnitY;

								// Define indices for two triangles 
								indices[indexOffset] = (ushort)(vertexIndex + 2);
								indices[indexOffset + 1] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 2] = (ushort)(vertexIndex + 0);

								indices[indexOffset + 3] = (ushort)(vertexIndex + 2);
								indices[indexOffset + 4] = (ushort)(vertexIndex + 3);
								indices[indexOffset + 5] = (ushort)(vertexIndex + 1);

								vertexIndex += 4;
								indexOffset += 6;
							}
							break;
						case BlockFace.PositiveY:
							if (GetBlockID(x, y + 1, z) == 0)
							{
								// Top face vertices
								vertices[vertexIndex] = new System.Numerics.Vector3(x, y + 1, z);
								vertices[vertexIndex + 1] = new System.Numerics.Vector3(x, y + 1, z + 1);
								vertices[vertexIndex + 2] = new System.Numerics.Vector3(x + 1, y + 1, z);
								vertices[vertexIndex + 3] = new System.Numerics.Vector3(x + 1, y + 1, z + 1);

								texcoords[vertexIndex] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);
								texcoords[vertexIndex + 1] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 2] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);
								texcoords[vertexIndex + 3] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);

								normals[vertexIndex] = System.Numerics.Vector3.UnitY;
								normals[vertexIndex + 1] = System.Numerics.Vector3.UnitY;
								normals[vertexIndex + 2] = System.Numerics.Vector3.UnitY;
								normals[vertexIndex + 3] = System.Numerics.Vector3.UnitY;

								// Define indices for two triangles (counterclockwise order for correct normals)
								indices[indexOffset] = (ushort)(vertexIndex);
								indices[indexOffset + 1] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 2] = (ushort)(vertexIndex + 2);

								indices[indexOffset + 3] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 4] = (ushort)(vertexIndex + 3);
								indices[indexOffset + 5] = (ushort)(vertexIndex + 2);			

								vertexIndex += 4;
								indexOffset += 6;
							}
							break;
						case BlockFace.NegativeZ:
							if (GetBlockID(x, y, z - 1) == 0)
							{
								// Front face vertices
								vertices[vertexIndex] = new System.Numerics.Vector3(x, y, z);
								vertices[vertexIndex + 1] = new System.Numerics.Vector3(x, y + 1, z);
								vertices[vertexIndex + 2] = new System.Numerics.Vector3(x + 1, y, z);
								vertices[vertexIndex + 3] = new System.Numerics.Vector3(x + 1, y + 1, z);

								texcoords[vertexIndex + 0] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 1] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);
								texcoords[vertexIndex + 2] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 3] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);

								normals[vertexIndex] = -System.Numerics.Vector3.UnitZ;
								normals[vertexIndex + 1] = -System.Numerics.Vector3.UnitZ;
								normals[vertexIndex + 2] = -System.Numerics.Vector3.UnitZ;
								normals[vertexIndex + 3] = -System.Numerics.Vector3.UnitZ;

								// Define indices for two triangles (counterclockwise order for correct normals)
								indices[indexOffset] = (ushort)(vertexIndex);
								indices[indexOffset + 1] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 2] = (ushort)(vertexIndex + 2);

								indices[indexOffset + 3] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 4] = (ushort)(vertexIndex + 3);
								indices[indexOffset + 5] = (ushort)(vertexIndex + 2);

								vertexIndex += 4;
								indexOffset += 6;
							}
							break;
						case BlockFace.PositiveZ:
							if (GetBlockID(x, y, z + 1) == 0)
							{
								// Back face vertices
								vertices[vertexIndex] = new System.Numerics.Vector3(x, y, z + 1);
								vertices[vertexIndex + 1] = new System.Numerics.Vector3(x, y + 1, z + 1);
								vertices[vertexIndex + 2] = new System.Numerics.Vector3(x + 1, y, z + 1);
								vertices[vertexIndex + 3] = new System.Numerics.Vector3(x + 1, y + 1, z + 1);

								texcoords[vertexIndex + 0] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 1] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);
								texcoords[vertexIndex + 2] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 3] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);

								normals[vertexIndex] = System.Numerics.Vector3.UnitZ;
								normals[vertexIndex + 1] = System.Numerics.Vector3.UnitZ;
								normals[vertexIndex + 2] = System.Numerics.Vector3.UnitZ;
								normals[vertexIndex + 3] = System.Numerics.Vector3.UnitZ;

								// Define indices for two triangles (counterclockwise order for correct normals)
								indices[indexOffset] = (ushort)(vertexIndex + 2);
								indices[indexOffset + 1] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 2] = (ushort)(vertexIndex + 0);

								indices[indexOffset + 3] = (ushort)(vertexIndex + 2);
								indices[indexOffset + 4] = (ushort)(vertexIndex + 3);
								indices[indexOffset + 5] = (ushort)(vertexIndex + 1);

								vertexIndex += 4;
								indexOffset += 6;
							}
							break;
						case BlockFace.NegativeX:
							if (GetBlockID(x - 1, y, z) == 0)
							{
								// Left face vertices
								vertices[vertexIndex] = new System.Numerics.Vector3(x, y, z);
								vertices[vertexIndex + 1] = new System.Numerics.Vector3(x, y + 1, z);
								vertices[vertexIndex + 2] = new System.Numerics.Vector3(x, y, z + 1);
								vertices[vertexIndex + 3] = new System.Numerics.Vector3(x, y + 1, z + 1);

								texcoords[vertexIndex + 0] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 1] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);
								texcoords[vertexIndex + 2] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 3] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);

								normals[vertexIndex] = -System.Numerics.Vector3.UnitX;
								normals[vertexIndex + 1] = -System.Numerics.Vector3.UnitX;
								normals[vertexIndex + 2] = -System.Numerics.Vector3.UnitX;
								normals[vertexIndex + 3] = -System.Numerics.Vector3.UnitX;

								// Define indices for two triangles (counterclockwise order for correct normals)
								indices[indexOffset] = (ushort)(vertexIndex + 2);
								indices[indexOffset + 1] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 2] = (ushort)(vertexIndex + 0);

								indices[indexOffset + 3] = (ushort)(vertexIndex + 2);
								indices[indexOffset + 4] = (ushort)(vertexIndex + 3);
								indices[indexOffset + 5] = (ushort)(vertexIndex + 1);

								vertexIndex += 4;
								indexOffset += 6;
							}
							break;
						case BlockFace.PositiveX:
							if (GetBlockID(x + 1, y, z) == 0)
							{
								// Right face vertices
								vertices[vertexIndex] = new System.Numerics.Vector3(x + 1, y, z);
								vertices[vertexIndex + 1] = new System.Numerics.Vector3(x + 1, y + 1, z);
								vertices[vertexIndex + 2] = new System.Numerics.Vector3(x + 1, y, z + 1);
								vertices[vertexIndex + 3] = new System.Numerics.Vector3(x + 1, y + 1, z + 1);

								texcoords[vertexIndex + 0] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 1] = new System.Numerics.Vector2((float)(atlasCoordinates.Item1 + textureAtlasBlockSize) / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);
								texcoords[vertexIndex + 2] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)(atlasCoordinates.Item2 + textureAtlasBlockSize) / textureAtlasSize);
								texcoords[vertexIndex + 3] = new System.Numerics.Vector2((float)atlasCoordinates.Item1 / textureAtlasSize, (float)atlasCoordinates.Item2 / textureAtlasSize);

								normals[vertexIndex] = System.Numerics.Vector3.UnitX;
								normals[vertexIndex + 1] = System.Numerics.Vector3.UnitX;
								normals[vertexIndex + 2] = System.Numerics.Vector3.UnitX;
								normals[vertexIndex + 3] = System.Numerics.Vector3.UnitX;

								// Define indices for two triangles (counterclockwise order for correct normals)
								indices[indexOffset] = (ushort)(vertexIndex);
								indices[indexOffset + 1] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 2] = (ushort)(vertexIndex + 2);

								indices[indexOffset + 3] = (ushort)(vertexIndex + 1);
								indices[indexOffset + 4] = (ushort)(vertexIndex + 3);
								indices[indexOffset + 5] = (ushort)(vertexIndex + 2);

								vertexIndex += 4;
								indexOffset += 6;
							}
							break;
					}
				}
			}
		}

		Raylib.UploadMesh(ref mesh, true);
		BlockFaceModel[(int)face] = Raylib.LoadModelFromMesh(mesh);

		Material material = Raylib.LoadMaterialDefault();
		material.Maps[(int)MaterialMapIndex.Albedo].Texture = BetaClient.Instance.terrainAtlas;
		BlockFaceModel[(int)face].Materials[0] = material;
		BlockFaceModel[(int)face].MaterialCount = 1;

		Raylib.SetModelMeshMaterial(ref BlockFaceModel[(int)face], 0, 0);
	}

	public void RegenerateBoundingBoxes()
	{
		BoundingBoxes.Clear();

		for (int x = 0; x < WorldConstants.ChunkWidth; x++)
		{
			for (int y = 0; y < WorldConstants.Height; y++)
			{
				for (int z = 0; z < WorldConstants.ChunkDepth; z++)
				{
					
					byte blockID = GetBlockID(x, y, z);
					if (blockID == 0) continue;

					/*
					List<byte> blockIDs = new List<byte>();
					blockIDs.Add(GetBlockID(x, y + 1, z));
					blockIDs.Add(GetBlockID(x, y - 1, z));
					blockIDs.Add(GetBlockID(x + 1, y, z));
					blockIDs.Add(GetBlockID(x - 1, y, z));
					blockIDs.Add(GetBlockID(x, y, z + 1));
					blockIDs.Add(GetBlockID(x, y, z - 1));

					for (int i = 0; i < blockIDs.Count; i++)
					{
						if (blockIDs[i] == 0)
						{
							BlockDefinition def = BlockRegistry.GetBlock(blockIDs[i]);
							if (def != null && def.Opaque) continue;
						}
					}
					*/
					
					BlockDefinition block = BlockRegistry.GetBlock(blockID);
					
					BoundingBox boundingBox = new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.One);

					if (block != null)
					{
						boundingBox = block.BoundingBox;
					}

					boundingBox.Min += new System.Numerics.Vector3(x + X * WorldConstants.ChunkWidth, y, z + Z * WorldConstants.ChunkDepth);
					boundingBox.Max += new System.Numerics.Vector3(x + X * WorldConstants.ChunkWidth, y, z + Z * WorldConstants.ChunkDepth);
					BoundingBoxes.Add(boundingBox);
				}
			}
		}
	}


	/// <summary>
	/// Converts Local Voxel Coordinates into an index into the internal arrays.
	/// </summary>
	/// <param name="coordinates">The Coordinates to convert</param>
	/// <returns>The index into the internal arrays.</returns>
	public static int LocalCoordinatesToIndex(int x, int y, int z)
	{
		return (x * WorldConstants.ChunkWidth + z) * WorldConstants.Height + y;
	}

	public static Vector3 IndexToLocalCoordinates(int index)
	{
		int x = index / (WorldConstants.ChunkWidth * WorldConstants.Height);
		int y = index % WorldConstants.Height;
		int z = (index / WorldConstants.Height) % WorldConstants.ChunkWidth;
		return new Vector3(x, y, z);
	}

    public byte GetBlockID(int x, int y, int z)
    {
		if (x < 0 || x >= WorldConstants.ChunkWidth || y < 0 || y >= WorldConstants.Height || z < 0 || z >= WorldConstants.ChunkDepth)
		{
			return 0;
		}
		return _blockIDs[LocalCoordinatesToIndex(x, y, z)];	
    }

	public void SetBlock(int x, int y, int z, byte blockID, byte metadata)
	{
		if (x < 0 || x >= WorldConstants.ChunkWidth || y < 0 || y >= WorldConstants.Height || z < 0 || z >= WorldConstants.ChunkDepth)
		{
			Debug.WriteLine($"Attempted to set block at invalid coordinates ({x}, {y}, {z})");
			return;
		}
		_blockIDs[LocalCoordinatesToIndex(x, y, z)] = blockID;
		RegenerateAllMesh();
	}
}