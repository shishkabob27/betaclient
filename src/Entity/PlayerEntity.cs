public class PlayerEntity : LivingEntity
{
	public const double Width = 0.6;
	public const double Height = 1.62;
	public const double Depth = 0.6;

	public string Username { get; set; }

    public override string ToString()
    {
        return $"PlayerEntity: {Username} ({EntityID}) at {Position} yaw {Yaw} pitch {Pitch}";
    }
}