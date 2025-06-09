namespace TownOfUsReworked.Modules;

/// <summary>
/// A wrapper for handling quick value conversions between <see cref="byte"/>, <see cref="int"/> and <see cref="float"/>.
/// </summary>
/// <param name="num">The value to be set.</param>
[Serializable]
public struct Number(float num) : IComparable, IFormattable, INetSerializable, INetDeserializable, IEquatable<Number>, IComparable<Number>
{
    public Number() : this(0) {}

    /// <summary>
    /// Gets the value.
    /// </summary>
    public float Value { get; private set; } = num;

    /// <summary>
    /// Implicitly converts to float.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>A float value.</returns>
    public static implicit operator float(Number number) => number.Value;

    /// <summary>
    /// Implicitly converts to int.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>An int value.</returns>
    public static implicit operator int(Number number) => (int)number.Value;

    /// <summary>
    /// Implicitly converts to byte.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <returns>A byte value.</returns>
    public static implicit operator byte(Number number) => (byte)number.Value;

    /// <summary>
    /// Implicitly converts to Number.
    /// </summary>
    /// <param name="num">The number to convert.</param>
    /// <returns>A Number instance.</returns>
    public static implicit operator Number(float num) => new(num);

    /// <inheritdoc cref="op_Implicit(float)"/>
    public static implicit operator Number(int num) => new(num);

    /// <summary>
    /// Equality check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if both values match</returns>
    public static bool operator ==(Number a, Number b) => a.Value == b.Value;

    /// <summary>
    /// Inequality check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if neither values match</returns>
    public static bool operator !=(Number a, Number b) => a.Value != b.Value;

    /// <summary>
    /// Order check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if <paramref name="a"/> is greater than <paramref name="b"/>.</returns>
    public static bool operator >(Number a, Number b) => a.Value > b.Value;

    /// <summary>
    /// Order check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if <paramref name="a"/> is greater than or equal <paramref name="b"/>.</returns>
    public static bool operator >=(Number a, Number b) => a.Value >= b.Value;

    /// <summary>
    /// Order check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if <paramref name="a"/> is lesser than <paramref name="b"/>.</returns>
    public static bool operator <(Number a, Number b) => a.Value < b.Value;

    /// <summary>
    /// Order check.
    /// </summary>
    /// <param name="a">Left.</param>
    /// <param name="b">Right.</param>
    /// <returns><c>true</c> if both values match</returns>
    /// <returns><c>true</c> if <paramref name="a"/> is lesser than or equal <paramref name="b"/>.</returns>
    public static bool operator <=(Number a, Number b) => a.Value <= b.Value;

    /// <inheritdoc/>
    public readonly byte[] GetBytes() => [.. RpcWriter.GetBytes(Value)];

    /// <inheritdoc/>
    public readonly override int GetHashCode() => Value.GetHashCode();

    /// <inheritdoc/>
    public readonly bool Equals(Number other) => Value == other.Value;

    /// <inheritdoc/>
    public readonly override bool Equals(object obj) => obj is Number number && Value == number.Value;

    /// <inheritdoc/>
    public readonly override string ToString() => Value.ToString();

    /// <inheritdoc/>
    public readonly string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);

    /// <inheritdoc/>
    public readonly int CompareTo(object obj) => obj switch
    {
        Number i => CompareTo(i),
        _ => Value.CompareTo(obj)
    };

    /// <inheritdoc/>
    public readonly int CompareTo(Number other) => Value.CompareTo(other.Value);

    /// <inheritdoc/>
    public void FromBytes(RpcReader reader) => Value = reader.ReadFloat();
}