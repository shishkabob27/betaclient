using System.Net.Sockets;

public class ClientNetwork
{
	TcpClient client;
	NetworkStream stream;

	byte[] incompletePacketBuffer = new byte[0]; // Buffer for incomplete packets

	bool connected = false;

	public bool LoginReceived = false;

	Dictionary<byte, Type> packetTypes = new Dictionary<byte, Type>
	{
		{ 0x00, typeof(KeepAlivePacket) },
		{ 0x01, typeof(LoginResponsePacket) },
		{ 0x02, typeof(HandshakeResponsePacket) },
		{ 0x03, typeof(ChatMessagePacket) },
		{ 0x04, typeof(TimeUpdatePacket) },
		{ 0x05, typeof(EntityEquipmentPacket) },
		{ 0x06, typeof(SpawnPositionPacket) },
		{ 0x08, typeof(UpdateHealthPacket) },
		{ 0x0A, typeof(PlayerGroundedPacket) },
		{ 0x0D, typeof(SetPlayerPositionPacket) },
		{ 0x12, typeof(AnimationPacket) },
		{ 0x14, typeof(SpawnPlayerPacket) },
		{ 0x15, typeof(SpawnItemPacket) },
		{ 0x16, typeof(CollectItemPacket) },
		{ 0x18, typeof(SpawnMobPacket) },
		{ 0x19, typeof(SpawnPaintingPacket) },
		{ 0x1C, typeof(EntityVelocityPacket) },
		{ 0x1D, typeof(DestroyEntityPacket) },
		{ 0x1F, typeof(EntityRelativeMovePacket) },
		{ 0x21, typeof(EntityLookAndRelativeMovePacket) },
		{ 0x22, typeof(EntityTeleportPacket) },
		{ 0x26, typeof(EntityStatusPacket) },
		{ 0x28, typeof(EntityMetadataPacket) },
		{ 0x32, typeof(ChunkPreamblePacket) },
		{ 0x33, typeof(ChunkDataPacket) },
		{ 0x34, typeof(BulkBlockChangePacket) },
		{ 0x35, typeof(BlockChangePacket) },
		{ 0x3D, typeof(SoundEffectPacket) },
		{ 0x46, typeof(EnvironmentStatePacket) },
		{ 0x47, typeof(LightningPacket) },
		{ 0x67, typeof(SetSlotPacket) },
		{ 0x68, typeof(WindowItemsPacket) },
		{ 0xC8, typeof(UpdateStatisticPacket) },
		{ 0x82, typeof(UpdateSignPacket) },
		{ 0xFF, typeof(DisconnectPacket) }
	};

	public void Initialize()
	{
		Connect();

		if (!IsConnected()) return;

		stream = client.GetStream();
	}

	void Connect(string ip = "127.0.0.1", int port = 25565)
	{
		client = new TcpClient();
		try
		{
			client.Connect(ip, port);
		}
		catch (SocketException e)
		{
			Logger.Error("Failed to connect to server: " + e.Message);
			return;
		}

		connected = true;

		Logger.Info("Connected to server");
	}

	public bool IsConnected()
	{
		if (client == null) return false;
		return connected && client.Connected && client.Client.Connected;
	}

	public bool ReadPackets()
    {
		if (!client.Connected)
		{
			Logger.Info("Disconnected from server");
			//break;
			return false;
		}

		if (stream.DataAvailable)
		{
			byte[] buffer = new byte[1024];
			int bytesRead = stream.Read(buffer, 0, buffer.Length);

			if (bytesRead == 0)
			{
				Logger.Info("Disconnected from server");
				//break;
				return false;
			}

			// Append new data to the incomplete packet buffer
			byte[] newBuffer = new byte[incompletePacketBuffer.Length + bytesRead];
			Buffer.BlockCopy(incompletePacketBuffer, 0, newBuffer, 0, incompletePacketBuffer.Length);
			Buffer.BlockCopy(buffer, 0, newBuffer, incompletePacketBuffer.Length, bytesRead);
			incompletePacketBuffer = newBuffer;

			// Attempt to process complete packets
			MemoryStream memoryStream = new MemoryStream(incompletePacketBuffer);
			MinecraftStream minecraftStream = new MinecraftStream(memoryStream);

			while (minecraftStream.Position < minecraftStream.Length)
			{
				long startPosition = minecraftStream.Position;
				if (!HandlePacket(minecraftStream))
				{
					// If the packet was incomplete, rewind and break
					minecraftStream.Position = startPosition;
					break;
				}
			}

			// Retain leftover data for the next read
			long leftoverDataLength = minecraftStream.Length - minecraftStream.Position;
			byte[] leftoverData = new byte[leftoverDataLength];
			Buffer.BlockCopy(incompletePacketBuffer, (int)minecraftStream.Position, leftoverData, 0, (int)leftoverDataLength);
			incompletePacketBuffer = leftoverData;
		}
		return true;
    }

	public void SendPacket(IPacket packet)
	{
		if (!IsConnected()) return;

		Stream buffer = new MemoryStream();
		MinecraftStream stream = new MinecraftStream(buffer);

		stream.WriteByte(packet.ID);
		packet.WritePacket(stream);

		byte[] data = ((MemoryStream)buffer).ToArray();
		stream.Write(data, 0, data.Length);

		//Logger.Debug($"Sending packet: {packet.ID.ToString("X2")} ({packet.GetType().Name})");
		//Logger.Debug($"Sending Buffer: {BitConverter.ToString(data)}");

		this.stream.Write(data, 0, data.Length);

		buffer.Close();
	}
	
	bool HandlePacket(MinecraftStream buffer)
    {
        try
        {
            // Read packet ID
            byte packetID = buffer.ReadUInt8();

            // Get packet type
            if (!packetTypes.TryGetValue(packetID, out Type packetType))
            {
                Logger.Debug("Unknown packet: " + packetID.ToString("X2"));
                Disconnect();
                return false;
            }

            // Create packet instance
            IPacket packet = (IPacket)Activator.CreateInstance(packetType);

            // Read and process packet
            packet.ReadPacket(buffer);
            packet.Action();
            return true;
        }
        catch (EndOfStreamException)
        {
            // Not enough data for the packet
            return false;
        }
}

	void Disconnect()
	{
		connected = false;
		client.Close();
		if (stream != null) stream.Close();


		Logger.Info("Disconnecting from server");
	}

	public void Shutdown()
	{
		Disconnect();
	}
}