public class PlayerEntity : LivingEntity
{
	public const double Width = 0.6;
	public const double Height = 1.62;
	public const double Depth = 0.6;

	public string Username { get; set; } = string.Empty;

	public PlayerEntity()
	{
		Health = 20;
		MaxHealth = 20;
		
		AccelerationDueToGravity = 1.6f;
		Drag = 0.4f;
		TerminalVelocity = 78.4f;
	}

    public override string ToString()
    {
        return $"PlayerEntity: {Username} ({EntityID}) at {Position} yaw {Yaw} pitch {Pitch}";
    }
}