public struct ChangeHeldItemPacket : IPacket
{
	public byte ID { get { return 0x10; } }

	private short _slot;

	public ChangeHeldItemPacket(short slot)
	{
		_slot = slot;
	}

	public short Slot { get => _slot; }

	public void ReadPacket(MinecraftStream stream)
	{
		_slot = stream.ReadInt16();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt16(_slot);
	}

	public void Action()
	{
		// Do nothing
	}
}