public interface IBlockModeler
{
	void RenderBlock(Chunk chunk, int x, int y, int z, ref List<System.Numerics.Vector3> vertices, ref List<System.Numerics.Vector2> texcoords, ref List<System.Numerics.Vector3> normals, ref List<Raylib_cs.Color> colors, ref List<ushort> indices);
}