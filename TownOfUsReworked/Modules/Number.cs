using System.Globalization;

namespace TownOfUsReworked.Modules;

[Serializable]
public readonly struct Number(float num) : IComparable, IConvertible, ISpanFormattable, IEquatable<Number>, IComparable<Number>
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

    public static Number operator ^(Number a, Number b) => Mathf.Pow(a.Value, b.Value);

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider provider) => Value.TryFormat(destination, out charsWritten, format,
        provider);

    public override bool Equals(object obj) => obj is Number number && Equals(number);

    public bool Equals(Number other) => Mathf.Approximately(Value, other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public static Number Parse(string value) => new(float.Parse(value));

    public TypeCode GetTypeCode() => TypeCode.Single;

    public bool ToBoolean(IFormatProvider provider) => ((IConvertible)Value).ToBoolean(provider);

    public byte ToByte(IFormatProvider provider) => ((IConvertible)Value).ToByte(provider);

    public char ToChar(IFormatProvider provider) => ((IConvertible)Value).ToChar(provider);

    public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)Value).ToDateTime(provider);

    public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)Value).ToDecimal(provider);

    public double ToDouble(IFormatProvider provider) => ((IConvertible)Value).ToDouble(provider);

    public short ToInt16(IFormatProvider provider) => ((IConvertible)Value).ToInt16(provider);

    public int ToInt32(IFormatProvider provider) => ((IConvertible)Value).ToInt32(provider);

    public long ToInt64(IFormatProvider provider) => ((IConvertible)Value).ToInt64(provider);

    public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)Value).ToSByte(provider);

    public float ToSingle(IFormatProvider provider) => ((IConvertible)Value).ToSingle(provider);

    public string ToString(IFormatProvider provider) => Value.ToString(provider);

    public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Value).ToType(conversionType, provider);

    public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)Value).ToUInt16(provider);

    public uint ToUInt32(IFormatProvider provider) => ((IConvertible)Value).ToUInt32(provider);

    public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)Value).ToUInt64(provider);

    public int CompareTo(object obj)
    {
        if (obj is Number other)
            return CompareTo(other);

        throw new ArgumentException("Object is not a Number");
    }

    public int CompareTo(Number other) => Value.CompareTo(other.Value);
}