/// <summary>
/// Sent by clients to update signs and by servers to notify clients of the change.
/// </summary>
public struct UpdateSignPacket : IPacket
{
    public byte ID { get { return 0x82; } }

    public int X;
    public short Y;
    public int Z;
    public string[] Text;

    public void ReadPacket(MinecraftStream stream)
    {
        X = stream.ReadInt32();
        Y = stream.ReadInt16();
        Z = stream.ReadInt32();
        Text = new string[4];
        Text[0] = stream.ReadString();
        Text[1] = stream.ReadString();
        Text[2] = stream.ReadString();
        Text[3] = stream.ReadString();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(X);
        stream.WriteInt16(Y);
        stream.WriteInt32(Z);
        stream.WriteString(Text[0]);
        stream.WriteString(Text[1]);
        stream.WriteString(Text[2]);
        stream.WriteString(Text[3]);
    }

	public void Action()
	{
		// Update the sign
		Logger.Debug($"Sign at {X}, {Y}, {Z} updated with text:");
		Logger.Debug($"  {Text[0]}");
		Logger.Debug($"  {Text[1]}");
		Logger.Debug($"  {Text[2]}");
		Logger.Debug($"  {Text[3]}");
	}
}