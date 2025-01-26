using System.Text;

public class MinecraftStream : Stream
{
	private Stream _baseStream;
	private Encoding StringEncoding = Encoding.BigEndianUnicode;

	public MinecraftStream(Stream baseStream)
	{
		_baseStream = baseStream;
	}

	public Stream BaseStream { get { return _baseStream; } }

	public override bool CanRead { get { return _baseStream.CanRead; } }

	public override bool CanSeek { get { return _baseStream.CanSeek; } }

	public override bool CanWrite { get { return _baseStream.CanWrite; } }

	public override void Flush()
	{
		_baseStream.Flush();
	}

	public override long Length
	{
		get { return _baseStream.Length; }
	}

	public override long Position
	{
		get { return _baseStream.Position; }
		set { _baseStream.Position = value; }
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		return _baseStream.Read(buffer, offset, count);
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return _baseStream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		_baseStream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		_baseStream.Write(buffer, offset, count);
	}

	public byte ReadUInt8()
	{
		int value = _baseStream.ReadByte();
		if (value == -1)
			throw new EndOfStreamException();
		return (byte)value;
	}

	public void WriteUInt8(byte value)
	{
		WriteByte(value);
	}

	public sbyte ReadInt8()
	{
		return (sbyte)ReadUInt8();
	}

	public void WriteInt8(sbyte value)
	{
		WriteUInt8((byte)value);
	}

	public ushort ReadUInt16()
	{
		return (ushort)(
			(ReadUInt8() << 8) |
			ReadUInt8());
	}

	public void WriteUInt16(ushort value)
	{
		Write(new[]
			{
				(byte)((value & 0xFF00) >> 8),
				(byte)(value & 0xFF)
			}, 0, 2);
	}

	public short ReadInt16()
	{
		return (short)ReadUInt16();
	}

	public void WriteInt16(short value)
	{
		WriteUInt16((ushort)value);
	}

	public uint ReadUInt32()
	{
		return (uint)(
			(ReadUInt8() << 24) |
			(ReadUInt8() << 16) |
			(ReadUInt8() << 8 ) |
			ReadUInt8());
	}

	public void WriteUInt32(uint value)
	{
		Write(new[]
			{
				(byte)((value & 0xFF000000) >> 24),
				(byte)((value & 0xFF0000) >> 16),
				(byte)((value & 0xFF00) >> 8),
				(byte)(value & 0xFF)
			}, 0, 4);
	}

	public int ReadInt32()
	{
		return (int)ReadUInt32();
	}

	public void WriteInt32(int value)
	{
		WriteUInt32((uint)value);
	}

	public ulong ReadUInt64()
	{
		return unchecked(
			((ulong)ReadUInt8() << 56) |
			((ulong)ReadUInt8() << 48) |
			((ulong)ReadUInt8() << 40) |
			((ulong)ReadUInt8() << 32) |
			((ulong)ReadUInt8() << 24) |
			((ulong)ReadUInt8() << 16) |
			((ulong)ReadUInt8() << 8)  |
			(ulong)ReadUInt8());
	}

	public void WriteUInt64(ulong value)
	{
		Write(new[]
			{
				(byte)((value & 0xFF00000000000000) >> 56),
				(byte)((value & 0xFF000000000000) >> 48),
				(byte)((value & 0xFF0000000000) >> 40),
				(byte)((value & 0xFF00000000) >> 32),
				(byte)((value & 0xFF000000) >> 24),
				(byte)((value & 0xFF0000) >> 16),
				(byte)((value & 0xFF00) >> 8),
				(byte)(value & 0xFF)
			}, 0, 8);
	}

	public long ReadInt64()
	{
		return (long)ReadUInt64();
	}

	public void WriteInt64(long value)
	{
		WriteUInt64((ulong)value);
	}

	public byte[] ReadUInt8Array(int length)
	{
		var result = new byte[length];
		if (length == 0) return result;
		int n = length;
		while (n > 0)
		{
			int read = _baseStream.Read(result, length - n, n);
			if (read == 0)
				throw new EndOfStreamException();
			n -= read;
		}
		return result;
	}

	public void WriteUInt8Array(byte[] value)
	{
		Write(value, 0, value.Length);
	}

	public void WriteUInt8Array(byte[] value, int offset, int count)
	{
		Write(value, offset, count);
	}

	public sbyte[] ReadInt8Array(int length)
	{
		return (sbyte[])(Array)ReadUInt8Array(length);
	}

	public void WriteInt8Array(sbyte[] value)
	{
		Write((byte[])(Array)value, 0, value.Length);
	}

	public ushort[] ReadUInt16Array(int length)
	{
		var result = new ushort[length];
		if (length == 0) return result;
		for (int i = 0; i < length; i++)
			result[i] = ReadUInt16();
		return result;
	}

	public void WriteUInt16Array(ushort[] value)
	{
		for (int i = 0; i < value.Length; i++)
			WriteUInt16(value[i]);
	}

	public short[] ReadInt16Array(int length)
	{
		return (short[])(Array)ReadUInt16Array(length);
	}

	public void WriteInt16Array(short[] value)
	{
		WriteUInt16Array((ushort[])(Array)value);
	}

	public uint[] ReadUInt32Array(int length)
	{
		var result = new uint[length];
		if (length == 0) return result;
		for (int i = 0; i < length; i++)
			result[i] = ReadUInt32();
		return result;
	}

	public void WriteUInt32Array(uint[] value)
	{
		for (int i = 0; i < value.Length; i++)
			WriteUInt32(value[i]);
	}

	public int[] ReadInt32Array(int length)
	{
		return (int[])(Array)ReadUInt32Array(length);
	}

	public void WriteInt32Array(int[] value)
	{
		WriteUInt32Array((uint[])(Array)value);
	}

	public ulong[] ReadUInt64Array(int length)
	{
		var result = new ulong[length];
		if (length == 0) return result;
		for (int i = 0; i < length; i++)
			result[i] = ReadUInt64();
		return result;
	}

	public void WriteUInt64Array(ulong[] value)
	{
		for (int i = 0; i < value.Length; i++)
			WriteUInt64(value[i]);
	}

	public long[] ReadInt64Array(int length)
	{
		return (long[])(Array)ReadUInt64Array(length);
	}

	public void WriteInt64Array(long[] value)
	{
		WriteUInt64Array((ulong[])(Array)value);
	}


	public float ReadSingle()
	{
		byte[] bytes = ReadUInt8Array(4);
		Array.Reverse(bytes);
		return BitConverter.ToSingle(bytes, 0);
	}

	public void WriteSingle(float value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		Array.Reverse(bytes);
		WriteUInt8Array(bytes);
	}

	public double ReadDouble()
	{
		byte[] bytes = ReadUInt8Array(8);
		Array.Reverse(bytes);
		return BitConverter.ToDouble(bytes, 0);
	}

	public void WriteDouble(double value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		Array.Reverse(bytes);
		WriteUInt8Array(bytes);
	}

	public bool ReadBoolean()
	{
		return ReadUInt8() != 0;
	}

	public void WriteBoolean(bool value)
	{
		WriteUInt8(value ? (byte)1 : (byte)0);
	}

	public string ReadString()
	{
		short length = ReadInt16();
		if (length == 0) return string.Empty;
		var data = ReadUInt8Array(length * 2);
		return StringEncoding.GetString(data);
	}

	public void WriteString(string value)
	{
		WriteInt16((short)value.Length);
		if (value.Length > 0)
			WriteUInt8Array(StringEncoding.GetBytes(value));
	}

	// TODO: string8 is modified UTF-8, which has a special representation of the NULL character
	public string ReadString8()
	{
		short length = ReadInt16();
		if (length == 0) return string.Empty;
		var data = ReadUInt8Array((int)length);
		return Encoding.UTF8.GetString(data);
	}

	public void WriteString8(string value)
	{
		WriteInt16((short)value.Length);
		if (value.Length > 0)
			WriteUInt8Array(Encoding.UTF8.GetBytes(value));
	}
}