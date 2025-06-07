using System.Diagnostics;
using System.IO.Compression;
using System.Numerics;
using Silk.NET.OpenGL;

public class Chunk
{
	public int X, Z;
	public bool HasRecivedData = false;
	public byte[] _blockIDs;
	public NybbleArray Metadata { get; private set; }
	public NybbleArray BlockLight { get; private set; }
	public NybbleArray SkyLight { get; private set; }

	// OpenGL mesh data
	public uint vertexArrayObject = 0;
	public uint vertexBufferObject = 0;
	public uint elementBufferObject = 0;
	public int indexCount = 0;

	// Separate meshes for opaque and transparent blocks
	public uint opaqueVAO = 0;
	public uint opaqueVBO = 0;
	public uint opaqueEBO = 0;
	public int opaqueIndexCount = 0;

	public uint transparentVAO = 0;
	public uint transparentVBO = 0;
	public uint transparentEBO = 0;
	public int transparentIndexCount = 0;

	public List<BoundingBox> BoundingBoxes = new List<BoundingBox>();

	// Vertex structure for OpenGL
	public struct Vertex
	{
		public Vector3 Position;
		public Vector2 TexCoord;
		public Vector3 Normal;
		public Vector4 Color;

		public Vertex(Vector3 position, Vector2 texCoord, Vector3 normal, Vector4 color)
		{
			Position = position;
			TexCoord = texCoord;
			Normal = normal;
			Color = color;
		}
	}

	public Chunk(int x, int z)
	{
		X = x;
		Z = z;
	}

	public void UpdateChunkData(ChunkDataPacket packet)
	{
        Console.WriteLine("Updating chunk data");
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

		var gl = BetaClient.Instance.gl;

		// Create separate lists for opaque and transparent blocks
		List<Vertex> verticesOpaque = new List<Vertex>();
		List<uint> indicesOpaque = new List<uint>();

		List<Vertex> verticesTransparent = new List<Vertex>();
		List<uint> indicesTransparent = new List<uint>();

		// Debug: Count block types
		Dictionary<byte, int> blockCounts = new Dictionary<byte, int>();

		for (int x = 0; x < WorldConstants.ChunkWidth; x++)
		{
			for (int y = 0; y < WorldConstants.Height; y++)
			{
				for (int z = 0; z < WorldConstants.ChunkDepth; z++)
				{
					byte blockID = GetBlockID(x, y, z);
					
					// Count block types for debugging
					if (!blockCounts.ContainsKey(blockID))
						blockCounts[blockID] = 0;
					blockCounts[blockID]++;
					
					if (blockID == 0) continue;
					
					BlockDefinition blockDef = BlockRegistry.GetBlock(blockID);
					
					// Select appropriate mesh based on opacity
					List<Vertex> vertices;
					List<uint> indices;
					
					if (blockDef.Opaque)
					{
						vertices = verticesOpaque;
						indices = indicesOpaque;
					}
					else
					{
						vertices = verticesTransparent;
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

					modeler.RenderBlock(this, x, y, z, GetMetadata(x, y, z), GetBlockLight(x, y, z), GetSkyLight(x, y, z), ref vertices, ref indices);
				}
			}
		}

		// Debug: Print block counts
		Console.WriteLine($"Chunk ({X}, {Z}) block counts:");
		foreach (var kvp in blockCounts.OrderBy(x => x.Key))
		{
			if (kvp.Value > 0)
				Console.WriteLine($"  Block ID {kvp.Key}: {kvp.Value} blocks");
		}

		// Create OpenGL objects for opaque mesh
		if (verticesOpaque.Count > 0)
		{
			if (opaqueVAO == 0)
			{
				opaqueVAO = gl.GenVertexArray();
				opaqueVBO = gl.GenBuffer();
				opaqueEBO = gl.GenBuffer();
			}

			gl.BindVertexArray(opaqueVAO);
			
			// Upload vertices
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, opaqueVBO);
			fixed (Vertex* vertexPtr = verticesOpaque.ToArray())
			{
				gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticesOpaque.Count * sizeof(Vertex)), vertexPtr, BufferUsageARB.StaticDraw);
			}

			// Upload indices
			gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, opaqueEBO);
			fixed (uint* indexPtr = indicesOpaque.ToArray())
			{
				gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicesOpaque.Count * sizeof(uint)), indexPtr, BufferUsageARB.StaticDraw);
			}

			// Set up vertex attributes
			// Position (location 0)
			gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);
			gl.EnableVertexAttribArray(0);

			// TexCoord (location 1)
			gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));
			gl.EnableVertexAttribArray(1);

			// Normal (location 2)
			gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));
			gl.EnableVertexAttribArray(2);

			// Color (location 3)
			gl.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(8 * sizeof(float)));
			gl.EnableVertexAttribArray(3);

			opaqueIndexCount = indicesOpaque.Count;
			
			// Debug: Print some vertex positions
			if (verticesOpaque.Count > 0)
			{
				Console.WriteLine($"Chunk ({X}, {Z}) opaque mesh: {verticesOpaque.Count} vertices, {indicesOpaque.Count} indices");
				Console.WriteLine($"First vertex position: {verticesOpaque[0].Position}");
				Console.WriteLine($"Last vertex position: {verticesOpaque[verticesOpaque.Count - 1].Position}");
			}
		}

		// Create OpenGL objects for transparent mesh
		if (verticesTransparent.Count > 0)
		{
			if (transparentVAO == 0)
			{
				transparentVAO = gl.GenVertexArray();
				transparentVBO = gl.GenBuffer();
				transparentEBO = gl.GenBuffer();
			}

			gl.BindVertexArray(transparentVAO);
			
			// Upload vertices
			gl.BindBuffer(BufferTargetARB.ArrayBuffer, transparentVBO);
			fixed (Vertex* vertexPtr = verticesTransparent.ToArray())
			{
				gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(verticesTransparent.Count * sizeof(Vertex)), vertexPtr, BufferUsageARB.StaticDraw);
			}

			// Upload indices
			gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, transparentEBO);
			fixed (uint* indexPtr = indicesTransparent.ToArray())
			{
				gl.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(indicesTransparent.Count * sizeof(uint)), indexPtr, BufferUsageARB.StaticDraw);
			}

			// Set up vertex attributes (same as opaque)
			gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);
			gl.EnableVertexAttribArray(0);

			gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));
			gl.EnableVertexAttribArray(1);

			gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));
			gl.EnableVertexAttribArray(2);

			gl.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(8 * sizeof(float)));
			gl.EnableVertexAttribArray(3);

			transparentIndexCount = indicesTransparent.Count;
		}

		gl.BindVertexArray(0);
	}

	public unsafe void RenderOpaque()
	{
		if (opaqueIndexCount <= 0) return;

		var gl = BetaClient.Instance.gl;
		
		if (opaqueVAO == 0)
		{
			Console.WriteLine($"ERROR: Chunk ({X}, {Z}) has no VAO but has {opaqueIndexCount} indices!");
			return;
		}
		
		gl.BindVertexArray(opaqueVAO);
		
		// Check if VAO binding succeeded
		gl.GetInteger(GetPName.VertexArrayBinding, out int boundVAO);
		if (boundVAO != opaqueVAO)
		{
			Console.WriteLine($"ERROR: Failed to bind VAO {opaqueVAO} for chunk ({X}, {Z})");
			return;
		}
		
		gl.DrawElements(PrimitiveType.Triangles, (uint)opaqueIndexCount, DrawElementsType.UnsignedInt, (void*)0);
		
		// Check for errors after draw call
		var error = gl.GetError();
		if (error != GLEnum.NoError)
		{
			Console.WriteLine($"OpenGL Error in chunk ({X}, {Z}) DrawElements: {error}");
		}
	}

	public unsafe void RenderTransparent()
	{
		if (transparentIndexCount <= 0) return;

		var gl = BetaClient.Instance.gl;
		gl.BindVertexArray(transparentVAO);
		gl.DrawElements(PrimitiveType.Triangles, (uint)transparentIndexCount, DrawElementsType.UnsignedInt, (void*)0);
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
					
					BoundingBox boundingBox = new BoundingBox(Vector3.Zero, Vector3.One);

					if (block != null)
					{
						// Convert from Raylib BoundingBox to our custom BoundingBox
						// This assumes the BlockDefinition has been updated accordingly
						// boundingBox = block.BoundingBox;
					}

					//if bounding box is zero, then skip this block
					if (boundingBox.Min == Vector3.Zero && boundingBox.Max == Vector3.Zero) continue;

					boundingBox.Min += new Vector3(x + X * WorldConstants.ChunkWidth, y, z + Z * WorldConstants.ChunkDepth);
					boundingBox.Max += new Vector3(x + X * WorldConstants.ChunkWidth, y, z + Z * WorldConstants.ChunkDepth);
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

	// Cleanup method to free OpenGL resources
	public void Dispose()
	{
		var gl = BetaClient.Instance.gl;
		
		if (opaqueVAO != 0)
		{
			gl.DeleteVertexArray(opaqueVAO);
			gl.DeleteBuffer(opaqueVBO);
			gl.DeleteBuffer(opaqueEBO);
		}

		if (transparentVAO != 0)
		{
			gl.DeleteVertexArray(transparentVAO);
			gl.DeleteBuffer(transparentVBO);
			gl.DeleteBuffer(transparentEBO);
		}
	}
}