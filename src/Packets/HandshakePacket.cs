public struct HandshakeRequestPacket : IPacket
{
    public byte ID { get { return 0x02; } }

    public HandshakeRequestPacket(string username)
    {
        Username = username;
    }

    public string Username;

    public void ReadPacket(MinecraftStream stream)
    {
        //Client doesn't receive this packet
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteString(Username);
    }

    public void Action()
    {
        
    }
}

public struct HandshakeResponsePacket : IPacket
{
    public byte ID { get { return 0x02; } }

    public HandshakeResponsePacket(string connectionHash)
    {
        ConnectionHash = connectionHash;
    }

    /// <summary>
    /// Set to "-" for offline mode servers. Online mode beta servers are obsolete.
    /// </summary>
    public string ConnectionHash;

    public void ReadPacket(MinecraftStream stream)
    {
        ConnectionHash = stream.ReadString();
    }

    public void WritePacket(MinecraftStream stream)
    {
        //Client doesn't send this packet
    }

    public void Action()
    {
        Logger.Debug($"Connection Hash: {ConnectionHash}");

        //send login packet
        LoginRequestPacket loginPacket = new LoginRequestPacket(14, BetaClient.Instance.Username);
        BetaClient.Instance.clientNetwork.SendPacket(loginPacket);
    }
}