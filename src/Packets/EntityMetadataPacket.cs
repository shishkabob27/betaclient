public struct EntityMetadataPacket : IPacket
{
    public byte ID { get { return 0x28; } }

    public int EntityID;
    public MetadataDictionary Metadata;

    public EntityMetadataPacket(int entityID, MetadataDictionary metadata)
    {
        EntityID = entityID;
        Metadata = metadata;
    }

    public void ReadPacket(MinecraftStream stream)
    {
        EntityID = stream.ReadInt32();
        Metadata = MetadataDictionary.FromStream(stream);
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(EntityID);
        Metadata.WriteTo(stream);
    }

	public void Action()
	{
		Logger.Debug($"EntityMetadataPacket EntityID: {EntityID}");
	}
}