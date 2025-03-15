namespace TownOfUsReworked.Modules;

[Serializable]
public readonly struct Number(float num)
    : IFormattable, IEquatable<Number>
{
    public float Value { get; } = num;

    public static implicit operator float(Number number) => number.Value;

    public static implicit operator int(Number number) => (int)number.Value;

    public static implicit operator byte(Number number) => (byte)number.Value;

    public static implicit operator Number(float num) => new(num);

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