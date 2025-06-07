using System.Diagnostics;
using System.Numerics;

/// <summary>
/// Sent by servers to set the position and look of the player. Can be used to teleport players.
/// </summary>
public struct SetPlayerPositionPacket : IPacket
{
    public byte ID { get { return 0x0D; } }

    public SetPlayerPositionPacket(double x, double y, double stance, double z, float yaw, float pitch, bool onGround)
    {
        X = x;
        Y = y;
        Z = z;
        Stance = stance;
        Yaw = yaw;
        Pitch = pitch;
        OnGround = onGround;
    }

    public double X, Y, Z;
    public double Stance;
    public float Yaw, Pitch;
    public bool OnGround;

    public void ReadPacket(MinecraftStream stream)
    {
        Debug.WriteLine("SetPlayerPositionPacket.ReadPacket");
        X = stream.ReadDouble();
        Stance = stream.ReadDouble();
        Y = stream.ReadDouble();
        Z = stream.ReadDouble();
        Yaw = stream.ReadSingle();
        Pitch = stream.ReadSingle();
        OnGround = stream.ReadBoolean();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteDouble(X);
        stream.WriteDouble(Y);
        stream.WriteDouble(Stance);
        stream.WriteDouble(Z);
        stream.WriteSingle(Yaw);
        stream.WriteSingle(Pitch);
        stream.WriteBoolean(OnGround);
    }

	public void Action()
	{
        BetaClient.Game.World.ReceivedFirstPlayerPosition = true;
		Logger.Debug($"Player position: {X}, {Y}, {Z}");
		Logger.Debug($"Stance: {Stance}");
		Logger.Debug($"Yaw: {Yaw}");
		Logger.Debug($"Pitch: {Pitch}");
		Logger.Debug($"On ground: {OnGround}");

        if (BetaClient.Game.World.PlayerID != -1)
        {
            PlayerEntity player = BetaClient.Game.World.GetPlayer();
            player.Position = new Vector3((float)X, (float)Y, (float)Z);
            Logger.Debug($"Setting player: {player.EntityID} position to {player.Position}");
        }
        else
        {
            Logger.Warn("Received SetPlayerPositionPacket but player entity not found");
        }

        //send back to server
        BetaClient.Instance.clientNetwork.SendPacket(this);
	}
}