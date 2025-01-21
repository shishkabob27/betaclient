/// <summary>
/// Updates the player on changes to or status of the environment.
/// </summary>
public struct EnvironmentStatePacket : IPacket
{
	public enum EnvironmentState
	{
		InvalidBed = 0,
		BeginRaining = 1,
		EndRaining = 2
	}

	public byte ID { get { return 0x46; } }

	public EnvironmentState State;

	public void ReadPacket(MinecraftStream stream)
	{
		State = (EnvironmentState)stream.ReadInt8();
	}

	public void WritePacket(MinecraftStream stream)
	{
		stream.WriteInt8((sbyte)State);
	}

	public void Action()
	{
		switch (State)
		{
			case EnvironmentState.InvalidBed:
				Logger.Debug("Invalid bed");
				break;
			case EnvironmentState.BeginRaining:
				Logger.Debug("Begin raining");
				break;
			case EnvironmentState.EndRaining:
				Logger.Debug("End raining");
				break;
		}
	}
}