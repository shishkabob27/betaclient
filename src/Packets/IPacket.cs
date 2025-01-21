public interface IPacket
{
    public byte ID { get; }
    void ReadPacket(MinecraftStream stream);
    void WritePacket(MinecraftStream stream);
    void Action();
}