using System.Reflection;

public class BlockRegistry
{
	public static readonly Dictionary<byte, BlockDefinition> Blocks = new Dictionary<byte, BlockDefinition>();

	public static void RegisterBlocks()
	{
		//get all types of BlockDefinition
		var blockTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(BlockDefinition)));
		
		foreach (var blockType in blockTypes)
		{
			//create an instance of the type
			var block = (BlockDefinition)Activator.CreateInstance(blockType);
			if (block.ID == 0) continue;
			//add the block to the dictionary
			Blocks.Add(block.ID, block);
		}
	}

	public static BlockDefinition GetBlock(byte id)
	{
		if (!Blocks.ContainsKey(id))
		{
			return null;
		}
		return Blocks[id];
	}
}