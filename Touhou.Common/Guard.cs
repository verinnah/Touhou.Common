using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Touhou.Common;

/// <summary>
/// Provides static methods for guarding conditions. This class cannot be inherited.
/// </summary>
internal static class Guard
{
	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/> or an <see cref="ArgumentException"/> if <paramref name="argument"/> is not writable.
	/// </summary>
	/// <param name="argument">The <see cref="Stream"/> argument to validate as non-<see langword="null"/>.</param>
	/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds. If you ommit this parameter, the name of <paramref name="argument"/> is used.</param>
	/// <exception cref="ArgumentException"><paramref name="argument"/> is not writable.</exception>
	/// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
	internal static void ThrowIfNullOrNotWritable([NotNull] Stream argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
	{
		ArgumentNullException.ThrowIfNull(argument);

		if (!argument.CanWrite)
		{
			throw new ArgumentException($"{paramName} must be a writable stream.", paramName);
		}
	}

	/// <summary>
	/// Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is <see langword="null"/> or an <see cref="ArgumentException"/> if <paramref name="argument"/> is not readable and seekable.
	/// </summary>
	/// <param name="argument">The <see cref="Stream"/> argument to validate as non-<see langword="null"/>.</param>
	/// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds. If you ommit this parameter, the name of <paramref name="argument"/> is used.</param>
	/// <exception cref="ArgumentException"><paramref name="argument"/> is not readable or seekable.</exception>
	/// <exception cref="ArgumentNullException"><paramref name="argument"/> is <see langword="null"/>.</exception>
	internal static void ThrowIfNullOrNotReadableAndSeekable([NotNull] Stream argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
	{
		ArgumentNullException.ThrowIfNull(argument);

		if (!argument.CanRead)
		{
			throw new ArgumentException($"{paramName} must be a readable stream.", paramName);
		}

		if (!argument.CanSeek)
		{
			throw new ArgumentException($"{paramName} must be a seekable stream.", paramName);
		}
	}
}
