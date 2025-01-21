using System.Text;
using System.Xml;
using fNbt;

public struct ItemStack : ICloneable, IEquatable<ItemStack>, IEquatable<NbtTag>
{
	/// <summary>
	/// Returns the hash code for this item stack.
	/// </summary>
	/// <returns></returns>
	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = _Id.GetHashCode();
			hashCode = (hashCode * 397) ^ _Count.GetHashCode();
			hashCode = (hashCode * 397) ^ _Metadata.GetHashCode();
			hashCode = (hashCode * 397) ^ Index;
			hashCode = (hashCode * 397) ^ (Nbt != null ? Nbt.GetHashCode() : 0);
			return hashCode;
		}
	}

	public static bool operator ==(ItemStack left, ItemStack right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ItemStack left, ItemStack right)
	{
		return !left.Equals(right);
	}

	/// <summary>
	/// Creates a new Item Stack from the given XML.
	/// </summary>
	/// <param name="stack">The XML to parse into an ItemStack.</param>
	public ItemStack(XmlNode stack)
	{
		XmlNode? idNode = stack.FirstChild;
		if (idNode is null)
			throw new ArgumentException("The given stack XML Node contains no children.");
		_Id = short.Parse(idNode.InnerText);

		XmlNode? countNode = idNode.NextSibling;
		if (countNode is null)
			throw new ArgumentException("The given stack XML Node has insufficient children.");
		if (_Id != -1)
			_Count = sbyte.Parse(countNode.InnerText);
		else
			_Count = 0;

		XmlNode? metadataNode = countNode.NextSibling;
		if (metadataNode is not null)
			_Metadata = short.Parse(metadataNode.InnerText);
		else
			_Metadata = 0;

		Nbt = null;
		Index = 0;
	}

	/// <summary>
	/// Creates a new item stack with the specified values.
	/// </summary>
	/// <param name="id">The item ID for the item stack.</param>
	public ItemStack(short id) : this()
	{
		_Id = id;
		_Count = (sbyte)(id != -1 ? 1 : 0);
		Metadata = 0;
		Nbt = null;
		Index = 0;
	}

	/// <summary>
	/// Creates a new item stack with the specified values.
	/// </summary>
	/// <param name="id">The item ID for the item stack.</param>
	/// <param name="count">The item count for the item stack.</param>
	public ItemStack(short id, sbyte count) : this(id)
	{
		_Count = (sbyte)(id != -1 ? count : 0);
	}

	/// <summary>
	/// Creates a new item stack with the specified values.
	/// </summary>
	/// <param name="id">The item ID for the item stack.</param>
	/// <param name="count">The item count for the item stack.</param>
	/// <param name="metadata">The metadata for the item stack.</param>
	public ItemStack(short id, sbyte count, short metadata) : this(id, count)
	{
		Metadata = metadata;
	}

	/// <summary>
	/// Creates a new item stack with the specified values.
	/// </summary>
	/// <param name="id">The item ID for the item stack.</param>
	/// <param name="count">The item count for the item stack.</param>
	/// <param name="metadata">The metadata for the item stack.</param>
	/// <param name="nbt">The NBT compound tag for the item stack.</param>
	public ItemStack(short id, sbyte count, short metadata, NbtCompound? nbt) : this(id, count, metadata)
	{
		Nbt = nbt;
		if (Count == 0)
		{
			ID = -1;
			Metadata = 0;
			Nbt = null;
		}
	}

	/// <summary>
	/// Creates and returns a new item stack read from a Minecraft stream.
	/// </summary>
	/// <param name="stream">The stream to read from.</param>
	/// <returns></returns>
	public static ItemStack FromStream(MinecraftStream stream)
	{
		var slot = ItemStack.EmptyStack;
		slot.ID = stream.ReadInt16();
		if (slot.Empty)
			return slot;
		slot.Count = stream.ReadInt8();
		slot.Metadata = stream.ReadInt16();
		var length = stream.ReadInt16();
		if (length == -1)
			return slot;
		slot.Nbt = new NbtCompound();
		var buffer = stream.ReadUInt8Array(length);
		var nbt = new NbtFile();
		nbt.LoadFromBuffer(buffer, 0, length, NbtCompression.GZip, null);
		slot.Nbt = nbt.RootTag;
		return slot;
	}

	/// <summary>
	/// Writes this item stack to a Minecraft stream.
	/// </summary>
	/// <param name="stream">The stream to write to.</param>
	public void WriteTo(MinecraftStream stream)
	{
		stream.WriteInt16(ID);
		if (Empty)
			return;
		stream.WriteInt8(Count);
		stream.WriteInt16(Metadata);
		if (Nbt == null)
		{
			stream.WriteInt16(-1);
			return;
		}
		var mStream = new MemoryStream();
		var file = new NbtFile(Nbt);
		file.SaveToStream(mStream, NbtCompression.GZip);
		stream.WriteInt16((short)mStream.Position);
		stream.WriteUInt8Array(mStream.GetBuffer());
	}

	/// <summary>
	/// Creates and returns a new item stack created from an NBT compound tag.
	/// </summary>
	/// <param name="compound">The compound tag to create the item stack from.</param>
	/// <returns></returns>
	public static ItemStack FromNbt(NbtCompound compound)
	{
		var s = ItemStack.EmptyStack;
		s.ID = compound.Get<NbtShort>("id").Value;
		s.Metadata = compound.Get<NbtShort>("Damage").Value;
		s.Count = (sbyte)compound.Get<NbtByte>("Count").Value;
		s.Index = compound.Get<NbtByte>("Slot").Value;
		if (compound.Get<NbtCompound>("tag") != null)
			s.Nbt = compound.Get<NbtCompound>("tag");
		return s;
	}

	/// <summary>
	/// Creates and returns a new NBT compound tag containing this item stack.
	/// </summary>
	/// <returns></returns>
	public NbtCompound ToNbt()
	{
		var c = new NbtCompound();
		c.Add(new NbtShort("id", ID));
		c.Add(new NbtShort("Damage", Metadata));
		c.Add(new NbtByte("Count", (byte)Count));
		c.Add(new NbtByte("Slot", (byte)Index));
		if (Nbt != null)
			c.Add(new NbtCompound("tag"));
		return c;
	}

	/// <summary>
	/// Gets whether this item stack is empty.
	/// </summary>
	//[NbtIgnore]
	public bool Empty
	{
		get { return ID == -1; }
	}

	/// <summary>
	/// Gets or sets the item ID for this item stack.
	/// </summary>
	public short ID
	{
		get { return _Id; }
		set
		{
			_Id = value;
			if (_Id == -1)
			{
				_Count = 0;
				Metadata = 0;
				Nbt = null;
			}
		}
	}

	/// <summary>
	/// Gets or sets the item count for this item stack.
	/// </summary>
	public sbyte Count
	{
		get { return _Count; }
		set
		{
			_Count = value;
			if (_Count == 0)
			{
				_Id = -1;
				Metadata = 0;
				Nbt = null;
			}
		}
	}

	/// <summary>
	/// Gets or sets the metadata for this item stack.
	/// </summary>
	public short Metadata
	{
		get { return _Metadata; }
		set { _Metadata = value; }
	}

	private short _Id;
	private sbyte _Count;
	private short _Metadata;

	/// <summary>
	/// The NBT compound tag for this item stack, if any.
	/// </summary>
	//[IgnoreOnNull]
	public NbtCompound? Nbt { get; set; }

	/// <summary>
	/// The index (slot) of this item stack in an inventory.
	/// </summary>
	//[NbtIgnore]
	public int Index;

	/// <summary>
	/// Returns the string representation of this item stack.
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		if (Empty)
			return "(Empty)";

		StringBuilder resultBuilder = new StringBuilder("ID: " + ID);

		if (Count != 1) resultBuilder.Append("; Count: " + Count);
		if (Metadata != 0) resultBuilder.Append("; Metadata: " + Metadata);
		if (Nbt != null) resultBuilder.Append(Environment.NewLine + Nbt.ToString());

		return "(" + resultBuilder.ToString() + ")";
	}

	/// <summary>
	/// Returns a clone of this item stack.
	/// </summary>
	/// <returns></returns>
	public object Clone()
	{
		return new ItemStack(ID, Count, Metadata, Nbt);
	}

	/// <summary>
	/// Gets an empty item stack.
	/// </summary>
	//[NbtIgnore]
	public static ItemStack EmptyStack
	{
		get
		{
			return new ItemStack(-1);
		}
	}

	/// <summary>
	/// Gets an ItemStack with a reduced number of items in it.
	/// </summary>
	/// <param name="n">The number of Items to remove from the ItemStack.</param>
	/// <returns>An ItemStack with the specified number of fewer items in it.</returns>
	public ItemStack GetReducedStack(int n)
	{
#if DEBUG
		if (n > Count)
			throw new ArgumentOutOfRangeException($"This ItemStack only contains {Count} items.  It cannot be reduced by {n}.");
#endif
		if (Count - n > 0)
			return new ItemStack(ID, (sbyte)(Count - n), Metadata, Nbt);
		else
			return EmptyStack;
	}

	/// <summary>
	/// Determines whether this item stack can merge with another.
	/// </summary>
	/// <param name="other">The other item stack.</param>
	/// <returns></returns>
	public bool CanMerge(ItemStack other)
	{
		if (this.Empty || other.Empty)
			return true;
		return _Id == other._Id && _Metadata == other._Metadata && Equals(Nbt, other.Nbt);
	}

	/// <summary>
	/// Determines whether this item stack and another object are equal.
	/// </summary>
	/// <param name="obj">The other object.</param>
	/// <returns></returns>
	public override bool Equals(object? obj)
	{
		if (obj is null) return false;

		if (obj is ItemStack)
			return Equals((ItemStack)obj);

		return obj is NbtTag && Equals((NbtTag)obj);
	}

	/// <summary>
	/// Determines whether this item stack and another are equal.
	/// </summary>
	/// <param name="other">The other item stack.</param>
	/// <returns></returns>
	public bool Equals(ItemStack other)
	{
		return _Id == other._Id && _Count == other._Count && _Metadata == other._Metadata && Index == other.Index && Equals(Nbt, other.Nbt);
	}

	#region IEquatable<NbtTag> & related
	public bool Equals(NbtTag? nbt)
	{
		NbtCompound? other = nbt as NbtCompound;
		if (other is null)
			return false;

		NbtShort id = other.Get<NbtShort>("id");
		if (id is null || _Id != id.Value)
			return false;

		NbtShort metadata = other.Get<NbtShort>("Damage");
		if (metadata is null || _Metadata != metadata.Value)
			return false;

		NbtByte count = other.Get<NbtByte>("Count");
		if (count is null || _Count != count.Value)
			return false;

		NbtByte slot = other.Get<NbtByte>("Slot");
		if (slot is null || Index != slot.Value)
			return false;

		// TODO: compare Nbt property

		return true;
	}

	public static bool operator==(ItemStack l, NbtTag? r)
	{
		return l.Equals(r);
	}

	public static bool operator !=(ItemStack l, NbtTag? r)
	{
		return !(l == r);
	}

	public static bool operator==(NbtTag? l, ItemStack r)
	{
		return (r == l);
	}

	public static bool operator !=(NbtTag? l, ItemStack r)
	{
		return !(r == l);
	}
	#endregion
}