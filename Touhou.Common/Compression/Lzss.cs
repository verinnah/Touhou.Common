using System.Runtime.CompilerServices;

namespace Touhou.Common.Compression;

/// <summary>
/// Provides static methods for (de)compression of data as implemented by the LZSS algorithm. This class cannot be inherited.
/// </summary>
public static class LZSS
{
	private const uint LZSS_MIN_MATCH = 3;
	// LZSS_MIN_MATCH + 4 bits
	private const uint LZSS_MAX_MATCH = 18;
	private const uint LZSS_DICTSIZE = 0x2000;
	private const uint LZSS_DICTSIZE_MASK = 0x1fff;

	private const uint HASH_NULL = 0;
	private const uint HASH_SIZE = 0x10000;

	/// <summary>
	/// Compresses <paramref name="inputStream"/> using LZSS into the specified <paramref name="outputStream"/>.
	/// </summary>
	/// <param name="inputStream">The stream that contains the data to compress.</param>
	/// <param name="inputSize">The size of the data.</param>
	/// <param name="outputStream">The stream into which the compressed data will be written.</param>
	/// <returns>The size of the compressed data.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="inputSize"/> is negative or zero.</exception>
	/// <exception cref="ArgumentNullException"><paramref name="inputStream"/> or <paramref name="outputStream"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException"><paramref name="inputStream"/> is not readable or seekable, or <paramref name="outputStream"/> is not writable.</exception>
	public static int Compress(Stream inputStream, int inputSize, Stream outputStream)
	{
		Guard.ThrowIfNullOrNotReadableAndSeekable(inputStream);
		Guard.ThrowIfNullOrNotWritable(outputStream);

		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(inputSize);

		uint dictHead = 1;
		int bytesRead = 0;
		uint waitingBytes = 0;
		byte[] dict = new byte[LZSS_DICTSIZE];

		// Fill the forward-looking buffer
		for (uint c = 0; c < LZSS_MAX_MATCH && c < inputSize; c++)
		{
			int ret = inputStream.ReadByte();

			if (ret == -1)
			{
				break;
			}

			bytesRead++;
			waitingBytes++;
			dict[dictHead + c] = (byte)ret;
		}

		uint dictHeadKey = GenerateKey(dict, dictHead);
		BitStream bitStream = new(outputStream);
		HashLinkedList hashes = new();

		while (waitingBytes != 0)
		{
			uint matchOffset = 0;
			uint matchLength = LZSS_MIN_MATCH - 1;

			// Find a good match
			for (uint offset = hashes.Hash[dictHeadKey]; offset != HASH_NULL && waitingBytes > matchLength; offset = hashes.Next[offset])
			{
				// First check a character further ahead to see if this match can be any longer than the current match
				if (dict[(dictHead + matchLength) & LZSS_DICTSIZE_MASK] == dict[(offset + matchLength) & LZSS_DICTSIZE_MASK])
				{
					// Then check the previous characters
					uint c = 0;
					for (; c < matchLength && dict[(dictHead + c) & LZSS_DICTSIZE_MASK] == dict[(offset + c) & LZSS_DICTSIZE_MASK]; c++) ;

					if (c < matchLength)
					{
						continue;
					}

					// Finally try to extend the match
					for (matchLength++; matchLength < waitingBytes && dict[(dictHead + matchLength) & LZSS_DICTSIZE_MASK] == dict[(offset + matchLength) & LZSS_DICTSIZE_MASK]; matchLength++) ;

					matchOffset = offset;
				}
			}

			// Write data to the output buffer
			if (matchLength < LZSS_MIN_MATCH)
			{
				matchLength = 1;

				bitStream.WriteBit(1);
				bitStream.Write(8, dict[dictHead]);
			}
			else
			{
				bitStream.WriteBit(0);
				bitStream.Write(13, matchOffset);
				bitStream.Write(4, matchLength - LZSS_MIN_MATCH);
			}

			// Add bytes to the dictionary
			for (uint c = 0; c < matchLength; c++)
			{
				uint offset2 = (dictHead + LZSS_MAX_MATCH) & LZSS_DICTSIZE_MASK;

				if (offset2 != HASH_NULL)
				{
					hashes.RemoveLast(GenerateKey(dict, offset2), offset2);
				}

				if (dictHead != HASH_NULL)
				{
					hashes.AddFirst(dictHeadKey, dictHead);
				}

				if (bytesRead < inputSize)
				{
					int ret = inputStream.ReadByte();

					if (ret == -1)
					{
						waitingBytes--;
					}
					else
					{
						dict[offset2] = (byte)ret;
						bytesRead++;
					}
				}
				else
				{
					waitingBytes--;
				}

				dictHead = (dictHead + 1) & LZSS_DICTSIZE_MASK;
				dictHeadKey = GenerateKey(dict, dictHead);
			}
		}

		bitStream.WriteBit(0);
		bitStream.Write(13, HASH_NULL);
		bitStream.Write(4, 0);

		bitStream.FinishByte();

		return (int)bitStream.Length;
	}

	/// <summary>
	/// Decompresses <paramref name="inputStream"/> compressed by LZSS into the specified <paramref name="outputStream"/>.
	/// </summary>
	/// <param name="inputStream">The stream that contains the data to decompress.</param>
	/// <param name="outputStream">The stream into which the decompressed data will be written.</param>
	/// <param name="outputSize">The size of the decompressed data.</param>
	/// <returns>The size of the decompressed data.</returns>
	/// <exception cref="ArgumentOutOfRangeException"><paramref name="outputSize"/> is negative or zero.</exception>
	/// <exception cref="ArgumentNullException"><paramref name="inputStream"/> or <paramref name="outputStream"/> is <see langword="null"/>.</exception>
	/// <exception cref="ArgumentException"><paramref name="inputStream"/> is not readable or seekable, or <paramref name="outputStream"/> is not writable.</exception>
	public static int Decompress(Stream inputStream, Stream outputStream, int outputSize)
	{
		Guard.ThrowIfNullOrNotReadableAndSeekable(inputStream);
		Guard.ThrowIfNullOrNotWritable(outputStream);

		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(outputSize);

		uint dictHead = 1;
		int bytesWritten = 0;
		byte[] dict = new byte[LZSS_DICTSIZE];
		BitStream bitStream = new(inputStream);

		while (bytesWritten < outputSize)
		{
			if (bitStream.Read(1) != 0)
			{
				byte c = (byte)bitStream.Read(8);
				outputStream.WriteByte(c);

				bytesWritten++;
				dict[dictHead] = c;
				dictHead = (dictHead + 1) & LZSS_DICTSIZE_MASK;
			}
			else
			{
				uint matchOffset = bitStream.Read(13);

				if (matchOffset == 0)
				{
					return bytesWritten;
				}

				uint matchLength = bitStream.Read(4) + LZSS_MIN_MATCH;

				for (uint i = 0; i < matchLength; i++)
				{
					byte c = dict[(matchOffset + i) & LZSS_DICTSIZE_MASK];
					outputStream.WriteByte(c);

					bytesWritten++;
					dict[dictHead] = c;
					dictHead = (dictHead + 1) & LZSS_DICTSIZE_MASK;
				}
			}
		}

		return bytesWritten;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static uint GenerateKey(ReadOnlySpan<byte> dict, uint head) => (uint)(((dict[(int)((head + 1) & LZSS_DICTSIZE_MASK)] << 8) | dict[(int)((head + 2) & LZSS_DICTSIZE_MASK)]) ^ (dict[(int)head] << 4));

	private sealed class HashLinkedList
	{
		internal uint[] Hash { get; } = new uint[HASH_SIZE];
		internal uint[] Next { get; } = new uint[LZSS_DICTSIZE];
		internal uint[] Previous { get; } = new uint[LZSS_DICTSIZE];

		internal void AddFirst(uint key, uint offset)
		{
			Next[offset] = Hash[key];
			Previous[offset] = HASH_NULL;

			// Update the previous pointer of the old head
			Previous[Hash[key]] = offset;
			Hash[key] = offset;
		}

		// This method either removes the last entry, or none at all
		internal void RemoveLast(uint key, uint offset)
		{
			Next[Previous[offset]] = HASH_NULL;

			// If the entry being removed was the head, clear the head
			if (Previous[offset] == HASH_NULL && Hash[key] == offset)
			{
				Hash[key] = HASH_NULL;
			}
		}
	}
}
