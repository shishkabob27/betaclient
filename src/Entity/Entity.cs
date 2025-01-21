public class Entity
{
	/// <summary>
	/// Gets or sets the Entity ID.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Note that the Entity ID can only be set once.
	/// </para>
	/// </remarks>
	public int EntityID { get; set; }

	/// <summary>
	/// Gets or sets the Position of the Entity.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Position specifies the centre of the Floor of the Entity's Bounding Box.
	/// </para>
	/// </remarks>
	public Vector3 Position { get; set; }


	/// <summary>
	/// Gets or sets the Yaw of the Entity.
	/// </summary>
	public float Yaw { get; set; }

	/// <summary>
	/// Gets or sets the Pitch of the Entity.
	/// </summary>
	public float Pitch { get; set; }
}