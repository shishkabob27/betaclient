public struct SpawnPaintingPacket : IPacket
{
    public enum PaintingDirection
    {
        NegativeZ = 0,
        NegativeX = 1,
        PositiveZ = 2,
        PositiveX = 3,
    }

    public byte ID { get { return 0x19; } }

    public int EntityID;
    public string PaintingName;
    public int X, Y, Z;
    public PaintingDirection Direction;

    public void ReadPacket(MinecraftStream stream)
    {
        EntityID = stream.ReadInt32();
        PaintingName = stream.ReadString();
        X = stream.ReadInt32();
        Y = stream.ReadInt32();
        Z = stream.ReadInt32();
        Direction = (PaintingDirection)stream.ReadInt32();
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(EntityID);
        stream.WriteString(PaintingName);
        stream.WriteInt32(X);
        stream.WriteInt32(Y);
        stream.WriteInt32(Z);
        stream.WriteInt32((int)Direction);
    }

    public void Action()
    {
        Logger.Debug($"Entity ID: {EntityID}");
        Logger.Debug($"Painting name: {PaintingName}");
        Logger.Debug($"Position: {X}, {Y}, {Z}");
        Logger.Debug($"Direction: {Direction}");   
    }
}