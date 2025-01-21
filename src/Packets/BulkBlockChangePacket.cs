using System.Diagnostics;

public struct BulkBlockChangePacket : IPacket
{
    public byte ID { get { return 0x34; } }

    public int ChunkX, ChunkZ;
    public LocalVoxelCoordinates[] Coordinates;
    public sbyte[] BlockIDs;
    public sbyte[] Metadata;

    public void ReadPacket(MinecraftStream stream)
    {
        ChunkX = stream.ReadInt32();
        ChunkZ = stream.ReadInt32();
        short length = stream.ReadInt16();
        Coordinates = new LocalVoxelCoordinates[length];
        for (int i = 0; i < length; i++)
        {
            ushort value = stream.ReadUInt16();
            Coordinates[i] = new LocalVoxelCoordinates(
                value >> 12 & 0xF,
                value & 0xFF,
                value >> 8 & 0xF);
        }
        BlockIDs = stream.ReadInt8Array(length);
        Metadata = stream.ReadInt8Array(length);
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(ChunkX);
        stream.WriteInt32(ChunkZ);
        stream.WriteInt16((short)Coordinates.Length);
        for (int i = 0; i < Coordinates.Length; i++)
        {
            var coord = Coordinates[i];
            // TODO: BUG: << occurs before &; so coord.X & coord.Z will be recorded as zeroes.
            //            Fix if this Packet type is ever used.
            //            Q: What coordinate system is in use?
            stream.WriteUInt16((ushort)((coord.X << 12 & 0xF) | (coord.Z << 8 & 0xF) | (coord.Y & 0xFF)));
        }
        stream.WriteInt8Array(BlockIDs);
        stream.WriteInt8Array(Metadata);
    }

	public void Action()
	{
		// Do nothing
	}
}