/// <summary>
/// Sent by servers to show the animation of an item entity being collected by a player.
/// </summary>
public struct CollectItemPacket : IPacket
{
    public byte ID { get { return 0x16; } }

    public int CollectedItemID;
    public int CollectorID;

    public CollectItemPacket(int collectedItemID, int collectorID)
    {
        CollectedItemID = collectedItemID;
        CollectorID = collectorID;
    }

    public void ReadPacket(MinecraftStream stream)
    {
        CollectedItemID = stream.ReadInt32();
        CollectorID = stream.ReadInt32();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(CollectedItemID);
        stream.WriteInt32(CollectorID);
    }

	public void Action()
	{
		// Do nothing
	}
}