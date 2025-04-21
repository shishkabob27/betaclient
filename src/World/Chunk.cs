using System.Diagnostics;
using System.IO.Compression;
using System.Numerics;
using Raylib_cs;
public class Chunk
{
	public int X, Z;
	public bool HasRecivedData = false;
	public byte[] _blockIDs;
	public NybbleArray Metadata { get; private set; }
	public NybbleArray BlockLight { get; private set; }
	public NybbleArray SkyLight { get; private set; }

	public Model model;

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

			if (packet.Width == WorldConstants.ChunkWidth && packet.Height == WorldConstants.Height && packet.Depth == WorldConstants.ChunkDepth)
			{
			}
			else
			{
				Logger.Warn($"small chunk: {packet.Width}x{packet.Height}x{packet.Depth}");
				return;
			}
			
	
			int blockCount = WorldConstants.ChunkDepth * WorldConstants.ChunkWidth * WorldConstants.Height * 5 / 2;
			_blockIDs = new byte[blockCount];
			
			ds.Read(_blockIDs, 0, blockCount);
			Metadata = new NybbleArray(ds, blockCount);
			BlockLight = new NybbleArray(ds, blockCount);
			SkyLight = new NybbleArray(ds, blockCount);

			RegenerateMesh();
			RegenerateBoundingBoxes();
        }
	}

	public unsafe void RegenerateMesh()
	{
		if (!HasRecivedData) return;

		// Create separate lists for opaque and non-opaque blocks
		List<System.Numerics.Vector3> verticesOpaque = new List<System.Numerics.Vector3>();
		List<System.Numerics.Vector2> texcoordsOpaque = new List<System.Numerics.Vector2>();
		List<System.Numerics.Vector3> normalsOpaque = new List<System.Numerics.Vector3>();
		List<Color> colorsOpaque = new List<Color>();
		List<ushort> indicesOpaque = new List<ushort>();

		List<System.Numerics.Vector3> verticesTransparent = new List<System.Numerics.Vector3>();
		List<System.Numerics.Vector2> texcoordsTransparent = new List<System.Numerics.Vector2>();
		List<System.Numerics.Vector3> normalsTransparent = new List<System.Numerics.Vector3>();
		List<Color> colorsTransparent = new List<Color>();
		List<ushort> indicesTransparent = new List<ushort>();

		for (int x = 0; x < WorldConstants.ChunkWidth; x++)
		{
			for (int y = 0; y < WorldConstants.Height; y++)
			{
				for (int z = 0; z < WorldConstants.ChunkDepth; z++)
				{
					if (GetBlockID(x, y, z) == 0) continue;
					
					byte blockID = GetBlockID(x, y, z);
					BlockDefinition blockDef = BlockRegistry.GetBlock(blockID);
					
					// Select appropriate mesh based on opacity
					List<System.Numerics.Vector3> vertices;
					List<System.Numerics.Vector2> texcoords;
					List<System.Numerics.Vector3> normals;
					List<Color> colors;
					List<ushort> indices;
					
					if (blockDef.Opaque)
					{
						vertices = verticesOpaque;
						texcoords = texcoordsOpaque;
						normals = normalsOpaque;
						colors = colorsOpaque;
						indices = indicesOpaque;
					}
					else
					{
						vertices = verticesTransparent;
						texcoords = texcoordsTransparent;
						normals = normalsTransparent;
						colors = colorsTransparent;
						indices = indicesTransparent;
					}
					
					IBlockModeler modeler;
					
					if (
						blockID == 6 ||
						blockID == 10 ||
						blockID == 31 ||
						blockID == 32 ||
						blockID == 37 ||
						blockID == 38 ||
						blockID == 39 ||
						blockID == 40 ||
						blockID == 83
					)
					{
						modeler = new QuadModeler();
					}
					else if (
						blockID == 44
					)
					{
						modeler = new SlabModeler();
					}
					else if (
						blockID == 50 ||
						blockID == 75 ||
						blockID == 76
					)
					{
						modeler = new TorchModeler();
					}
					else
					{
						modeler = new CubeModeler();
					}

					modeler.RenderBlock(this, x, y, z, GetMetadata(x, y, z), GetBlockLight(x, y, z), GetSkyLight(x, y, z), ref vertices, ref texcoords, ref normals, ref colors, ref indices);
				}
			}
		}

		// Create the model with multiple meshes
		int vertexCountOpaque = verticesOpaque.Count;
		int vertexCountTransparent = verticesTransparent.Count;

		// Create first mesh for opaque blocks
		Mesh meshOpaque = default;
		if (vertexCountOpaque > 0)
		{
			meshOpaque = new Mesh(vertexCountOpaque, indicesOpaque.Count / 2);
			meshOpaque.AllocVertices();
			meshOpaque.AllocTexCoords();
			meshOpaque.AllocNormals();
			meshOpaque.AllocColors();
			meshOpaque.AllocIndices();

			FillMeshData(meshOpaque, verticesOpaque, texcoordsOpaque, normalsOpaque, colorsOpaque, indicesOpaque);
			Raylib.UploadMesh(ref meshOpaque, true);
		}

		// Create second mesh for transparent blocks
		Mesh meshTransparent = default;
		if (vertexCountTransparent > 0)
		{
			meshTransparent = new Mesh(vertexCountTransparent, indicesTransparent.Count / 2);
			meshTransparent.AllocVertices();
			meshTransparent.AllocTexCoords();
			meshTransparent.AllocNormals();
			meshTransparent.AllocColors();
			meshTransparent.AllocIndices();

			FillMeshData(meshTransparent, verticesTransparent, texcoordsTransparent, normalsTransparent, colorsTransparent, indicesTransparent);
			Raylib.UploadMesh(ref meshTransparent, true);
		}

		// Create model with two meshes if both exist
		int meshCount = (vertexCountOpaque > 0 ? 1 : 0) + (vertexCountTransparent > 0 ? 1 : 0);
		if (meshCount == 0) return; // No meshes to create

		// Load the model with the first mesh
		if (vertexCountOpaque > 0)
		{
			model = Raylib.LoadModelFromMesh(meshOpaque);
		}
		else
		{
			model = Raylib.LoadModelFromMesh(meshTransparent);
		}

		// Adjust the model to support two meshes if needed
		if (meshCount == 2)
		{
			// Create a new model with two meshes
			model.MeshCount = 2;
			
			// You'll need to manage memory manually for this part
			// This is simplified and might need adjustment based on Raylib_cs implementation
			unsafe
			{
				Mesh* meshes = (Mesh*)Raylib.MemAlloc((uint)(sizeof(Mesh) * 2));
				meshes[0] = meshOpaque;
				meshes[1] = meshTransparent;
				
				if (model.Meshes != null)
				{
					Raylib.MemFree(model.Meshes);
				}
				
				model.Meshes = meshes;
				
				// Adjust materials array
				Material* materials = (Material*)Raylib.MemAlloc((uint)(sizeof(Material) * 2));
				
				// Setup materials
				Material materialOpaque = Raylib.LoadMaterialDefault();
				materialOpaque.Maps[(int)MaterialMapIndex.Albedo].Texture = BetaClient.Instance.terrainAtlas;
				materialOpaque.Maps[(int)MaterialMapIndex.Diffuse].Texture = BetaClient.Instance.terrainAtlas;
				
				Material materialTransparent = Raylib.LoadMaterialDefault();
				materialTransparent.Maps[(int)MaterialMapIndex.Albedo].Texture = BetaClient.Instance.terrainAtlas;
				materialTransparent.Maps[(int)MaterialMapIndex.Diffuse].Texture = BetaClient.Instance.terrainAtlas;
				
				materials[0] = materialOpaque;
				materials[1] = materialTransparent;
				
				if (model.Materials != null)
				{
					Raylib.MemFree(model.Materials);
				}
				
				model.Materials = materials;
				model.MaterialCount = 2;
				
				// Setup mesh material mapping
				int* meshMaterial = (int*)Raylib.MemAlloc((uint)(sizeof(int) * 2));
				meshMaterial[0] = 0; // First mesh uses first material
				meshMaterial[1] = 1; // Second mesh uses second material
				
				if (model.MeshMaterial != null)
				{
					Raylib.MemFree(model.MeshMaterial);
				}
				
				model.MeshMaterial = meshMaterial;
			}
		}
		else
		{
			// Just one mesh, setup the material as before
			Material material = Raylib.LoadMaterialDefault();
			material.Maps[(int)MaterialMapIndex.Albedo].Texture = BetaClient.Instance.terrainAtlas;
			material.Maps[(int)MaterialMapIndex.Diffuse].Texture = BetaClient.Instance.terrainAtlas;
			model.Materials[0] = material;
			model.MaterialCount = 1;
			Raylib.SetModelMeshMaterial(ref model, 0, 0);
		}
	}

	// Helper method to fill mesh data from lists
	private unsafe void FillMeshData(Mesh mesh, List<System.Numerics.Vector3> vertices, 
									List<System.Numerics.Vector2> texcoords, 
									List<System.Numerics.Vector3> normals, 
									List<Color> colors, 
									List<ushort> indices)
	{
		Span<System.Numerics.Vector3> verticesMeshSpan = mesh.VerticesAs<System.Numerics.Vector3>();
		Span<System.Numerics.Vector2> texcoordsSpan = mesh.TexCoordsAs<System.Numerics.Vector2>();
		Span<System.Numerics.Vector3> normalsSpan = mesh.NormalsAs<System.Numerics.Vector3>();
		Span<Color> colorsSpan = mesh.ColorsAs<Color>();
		Span<ushort> indicesSpan = mesh.IndicesAs<ushort>();

		//add all vertices to the mesh
		for (int i = 0; i < vertices.Count; i++)
		{
			verticesMeshSpan[i] = vertices[i];
		}

		//add all texcoords to the mesh
		for (int i = 0; i < texcoords.Count; i++)
		{
			texcoordsSpan[i] = texcoords[i];
		}

		//add all normals to the mesh
		for (int i = 0; i < normals.Count; i++)
		{
			normalsSpan[i] = normals[i];
		}

		//add all colors to the mesh
		for (int i = 0; i < colors.Count; i++)
		{
			colorsSpan[i] = colors[i];
		}

		//add all indices to the mesh
		for (int i = 0; i < indices.Count; i++)
		{
			indicesSpan[i] = indices[i];
		}
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
					
					List<byte> blockIDs = new List<byte>(){
						GetBlockID(x, y + 1, z),
						GetBlockID(x, y - 1, z),
						GetBlockID(x + 1, y, z),
						GetBlockID(x - 1, y, z),
						GetBlockID(x, y, z + 1),
						GetBlockID(x, y, z - 1)
					};

					bool hasAtLeastOneFaceVisible = false;

					for (int i = 0; i < blockIDs.Count; i++)
					{
						if (blockIDs[i] == 0)
						{
							hasAtLeastOneFaceVisible = true;
							continue;
						}

						BlockDefinition blockDef = BlockRegistry.GetBlock(blockIDs[i]);
						if (blockDef == null)
						{
							hasAtLeastOneFaceVisible = true;
							continue;
						}

						if (!blockDef.Opaque)
						{
							hasAtLeastOneFaceVisible = true;
							continue;
						}
					}

					if (!hasAtLeastOneFaceVisible) continue;
					
					
					BlockDefinition block = BlockRegistry.GetBlock(blockID);
					
					BoundingBox boundingBox = new BoundingBox(System.Numerics.Vector3.Zero, System.Numerics.Vector3.One);

					if (block != null)
					{
						boundingBox = block.BoundingBox;
					}

					//if bounding box is zero, then skip this block
					if (boundingBox.Min == System.Numerics.Vector3.Zero && boundingBox.Max == System.Numerics.Vector3.Zero) continue;

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
	}

	public byte GetMetadata(int x, int y, int z)
	{
		if (x < 0 || x >= WorldConstants.ChunkWidth || y < 0 || y >= WorldConstants.Height || z < 0 || z >= WorldConstants.ChunkDepth)
		{
			return 0;
		}
		return Metadata[LocalCoordinatesToIndex(x, y, z)];
	}

	public void SetMetadata(int x, int y, int z, byte metadata)
	{
		if (x < 0 || x >= WorldConstants.ChunkWidth || y < 0 || y >= WorldConstants.Height || z < 0 || z >= WorldConstants.ChunkDepth)
		{
			Debug.WriteLine($"Attempted to set metadata at invalid coordinates ({x}, {y}, {z})");
			return;
		}
		Metadata[LocalCoordinatesToIndex(x, y, z)] = metadata;
	}

	public byte GetBlockLight(int x, int y, int z)
	{
		if (x < 0 || x >= WorldConstants.ChunkWidth || y < 0 || y >= WorldConstants.Height || z < 0 || z >= WorldConstants.ChunkDepth)
		{
			return 0;
		}
		return BlockLight[LocalCoordinatesToIndex(x, y, z)];
	}

	public void SetBlockLight(int x, int y, int z, byte blockLight)
	{
		if (x < 0 || x >= WorldConstants.ChunkWidth || y < 0 || y >= WorldConstants.Height || z < 0 || z >= WorldConstants.ChunkDepth)
		{
			Debug.WriteLine($"Attempted to set block light at invalid coordinates ({x}, {y}, {z})");
			return;
		}
		BlockLight[LocalCoordinatesToIndex(x, y, z)] = blockLight;
	}

	public byte GetSkyLight(int x, int y, int z)
	{
		if (x < 0 || x >= WorldConstants.ChunkWidth || y < 0 || y >= WorldConstants.Height || z < 0 || z >= WorldConstants.ChunkDepth)
		{
			return 0;
		}
		return SkyLight[LocalCoordinatesToIndex(x, y, z)];
	}

	public void SetSkyLight(int x, int y, int z, byte skyLight)
	{
		if (x < 0 || x >= WorldConstants.ChunkWidth || y < 0 || y >= WorldConstants.Height || z < 0 || z >= WorldConstants.ChunkDepth)
		{
			Debug.WriteLine($"Attempted to set sky light at invalid coordinates ({x}, {y}, {z})");
			return;
		}
		SkyLight[LocalCoordinatesToIndex(x, y, z)] = skyLight;
	}
}