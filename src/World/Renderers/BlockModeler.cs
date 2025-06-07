public interface IBlockModeler
{
	void RenderBlock(Chunk chunk, int x, int y, int z, byte metadata, byte blocklight, byte skylight, ref List<Chunk.Vertex> vertices, ref List<uint> indices);
}