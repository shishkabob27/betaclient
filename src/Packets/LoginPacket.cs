/// <summary>
/// Sent by clients after the handshake to request logging into the world.
/// </summary>
public struct LoginRequestPacket : IPacket
{
    public byte ID { get { return 0x01; } }

    public LoginRequestPacket(int protocolVersion, string username)
    {
        ProtocolVersion = protocolVersion;
        Username = username;
    }

    public int ProtocolVersion;
    public string Username;

    public void ReadPacket(MinecraftStream stream)
    {
        //Client doesn't receive this packet
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(ProtocolVersion);
        stream.WriteString(Username);
        stream.WriteInt64(0); // Unused
        stream.WriteInt8(0);  // Unused
    }

    public void Action()
    {
        
    }
}

/// <summary>
/// Sent by the server to allow the player to spawn, with information about the world being spawned into.
/// </summary>
public struct LoginResponsePacket : IPacket
{
    public byte ID { get { return 0x01; } }

    public LoginResponsePacket(int entityID, long seed, sbyte dimension)
    {
        EntityID = entityID;
        Seed = seed;
        Dimension = dimension;
    }

    public int EntityID;
    public long Seed;
    public sbyte Dimension;

    public void ReadPacket(MinecraftStream stream)
    {
        EntityID = stream.ReadInt32();
        stream.ReadString(); // Unused
        Seed = stream.ReadInt64();
        Dimension = (sbyte)stream.ReadInt8();
    }

    public void WritePacket(MinecraftStream stream)
    {
        //Client doesn't send this packet
    }

    public void Action()
    {
        Logger.Debug($"Entity ID: {EntityID}");
        Logger.Debug($"Seed: {Seed}");
        Logger.Debug($"Dimension: {Dimension}");

        BetaClient.Game.World.Dimension = Dimension;

        //create player entity
        BetaClient.Game.World.PlayerID = EntityID;
        PlayerEntity player = new PlayerEntity();
        player.EntityID = EntityID;
        player.Username = BetaClient.Instance.Username;
        BetaClient.Game.World.AddEntity(player);

        BetaClient.Instance.clientNetwork.LoginReceived = true;

        //send ground packet
        //BetaClient.Instance.clientNetwork.SendPacket(new PlayerGroundedPacket(true));
    }
}