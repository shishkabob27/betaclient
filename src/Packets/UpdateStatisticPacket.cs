/// <summary>
/// Sent to update the client's list of player statistics.
/// </summary>
public struct UpdateStatisticPacket : IPacket
{
    public byte ID { get { return 0xC8; } }

    public int StatisticID;
    public sbyte Delta;

    public void ReadPacket(MinecraftStream stream)
    {
        StatisticID = stream.ReadInt32();
        Delta = stream.ReadInt8();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(StatisticID);
        stream.WriteInt8(Delta);
    }

	public void Action()
	{
		Logger.Debug($"UpdateStatistic: {StatisticID} {Delta}");
	}
}