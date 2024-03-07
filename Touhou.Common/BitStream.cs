using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Touhou.Common;

/// <summary>
/// Wraps over a stream to manipulate bits. This class cannot be inherited.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "I don't care")]
public sealed class BitStream
{
	/// <summary>
	/// Gets the length of the stream in bytes.
	/// </summary>
	public uint Length { get; private set; }

	private uint _bits;
	private uint _byteValue;
	private readonly Stream _stream;

	/// <summary>
	/// Creates a new instance of the <see cref="BitStream"/> class that wraps over <paramref name="stream"/>.
	/// </summary>
	/// <param name="stream">The stream to wrap.</param>
	/// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
	public BitStream(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream);

		_stream = stream;
	}

	/// <summary>
	/// Reads the specified number of <paramref name="bits"/> from the stream.
	/// </summary>
	/// <param name="bits">The number of bits to read.</param>
	public uint Read(uint bits)
	{
		if (bits > 25)
		{
			Debug.Assert(bits <= 32);

			uint r = Read(24);
			bits -= 24;

			return (r << (int)bits) | Read(bits);
		}

		while (bits > _bits)
		{
			_byteValue = (_byteValue << 8) | (byte)_stream.ReadByte();
			_bits += 8;
			Length++;
		}

		_bits -= bits;

		return (uint)((_byteValue >> (int)_bits) & ((1 << (int)bits) - 1));
	}

	/// <summary>
	/// Writes the specified <paramref name="bit"/> into the stream.
	/// </summary>
	/// <param name="bit">The bit to write.</param>
	public void WriteBit(uint bit)
	{
		_byteValue <<= 1;
		_byteValue |= bit & 1;

		if (++_bits == 8)
		{
			_stream.WriteByte((byte)_byteValue);

			_bits = 0;
			_byteValue = 0;

			Length++;
		}
	}

	/// <summary>
	/// Writes the specified number of <paramref name="bits"/> from <paramref name="data"/> into the stream.
	/// </summary>
	/// <param name="bits">The number of bits to write from <paramref name="data"/>.</param>
	/// <param name="data">The data from which to read.</param>
	public void Write(int bits, uint data)
	{
		if (bits > 32)
		{
			bits = 32;
		}

		for (int c = bits - 1; c >= 0; c--)
		{
			WriteBit((data >> c) & 1);
		}
	}

	/// <summary>
	/// Finishes writing remaining bits into the stream by filling the current byte with padding bits (zero-filling).
	/// </summary>
	public void FinishByte()
	{
		while (_bits != 0)
		{
			WriteBit(0);
		}
	}
}
