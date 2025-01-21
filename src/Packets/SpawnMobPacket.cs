using System.Diagnostics;

public struct SpawnMobPacket : IPacket
{
    public byte ID { get { return 0x18; } }

    public SpawnMobPacket(int entityId, sbyte type, int x, int y, int z, sbyte yaw, sbyte pitch, MetadataDictionary metadata)
    {
        EntityID = entityId;
        MobType = type;
        X = x;
        Y = y;
        Z = z;
        Yaw = yaw;
        Pitch = pitch;
        Metadata = metadata;
    }

    public int EntityID;
    public sbyte MobType;
    public int X, Y, Z;
    public sbyte Yaw, Pitch;
    public MetadataDictionary Metadata;

    public void ReadPacket(MinecraftStream stream)
    {
        EntityID = stream.ReadInt32();
        MobType = stream.ReadInt8();
        Logger.Debug($"Entity ID: {EntityID} Mob Type: {MobType}");
        X = stream.ReadInt32();
        Y = stream.ReadInt32();
        Z = stream.ReadInt32();
        Yaw = stream.ReadInt8();
        Pitch = stream.ReadInt8();
        Metadata = MetadataDictionary.FromStream(stream);
    }

    public void WritePacket(MinecraftStream stream)
    {
        stream.WriteInt32(EntityID);
        stream.WriteInt8(MobType);
        stream.WriteInt32(X);
        stream.WriteInt32(Y);
        stream.WriteInt32(Z);
        stream.WriteInt8(Yaw);
        stream.WriteInt8(Pitch);
        Metadata.WriteTo(stream);
    }

    public void Action()
    {
        Logger.Debug($"Entity ID: {EntityID}");
        Logger.Debug($"Mob type: {MobType}");
        Logger.Debug($"Position: {X}, {Y}, {Z}");
        Logger.Debug($"Yaw: {Yaw}");
        Logger.Debug($"Pitch: {Pitch}");
        Logger.Debug($"Metadata: {Metadata}");
    }
}