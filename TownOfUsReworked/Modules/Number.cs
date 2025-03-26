namespace TownOfUsReworked.Modules;

/// <summary>
/// A wrapper for handling quick value conversions between <see cref="byte"/>, <see cref="int"/> and <see cref="float"/>.
/// </summary>
/// <param name="num">The value to be set.</param>
[Serializable]
public readonly struct Number(float num) : IFormattable, IEquatable<Number>
{
    /// <summary>
    /// Gets the value.
    /// </summary>
    public float Value { get; } = num;

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

    public static bool operator ==(Number a, Number b) => a.Value == b.Value;

    public static bool operator !=(Number a, Number b) => a.Value != b.Value;

    public static bool operator >(Number a, Number b) => a.Value > b.Value;

    public static bool operator >=(Number a, Number b) => a.Value >= b.Value;

    public static bool operator <(Number a, Number b) => a.Value < b.Value;

    public static bool operator <=(Number a, Number b) => a.Value <= b.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public bool Equals(Number other) => Value == other.Value;

    public override bool Equals(object obj) => obj is Number number && Value == number.Value;

    public override string ToString() => Value.ToString();

    public string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);
}