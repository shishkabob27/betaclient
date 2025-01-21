/// <summary>
/// Used by clients to send messages and by servers to propegate messages to clients.
/// Note that the server is expected to include the username, i.e. <User> message, but the
/// client is not given the same expectation.
/// </summary>
public struct ChatMessagePacket : IPacket
{
    public byte ID { get { return 0x03; } }

    public ChatMessagePacket(string message)
    {
        Message = message;
    }

    public string Message;

    public void ReadPacket(MinecraftStream stream)
    {
        Message = stream.ReadString();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteString(Message);
    }

	public void Action()
	{
		Console.WriteLine(Message);

        BetaClient.Game.chatHistory.Add(new Tuple<string, int>(Message, 0));
	}
}