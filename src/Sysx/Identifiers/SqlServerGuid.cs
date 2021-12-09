﻿namespace Sysx.Identifiers;

/// <summary>
/// This type is a thin wrapper over Guid to create semi-sequential values by overriding the comparison methods to use SQL Server's unique sort order.
/// Using this type is appropriate when the field is a UNIQUEIDENTIFIER field in a SQL Server database.
/// </summary>
public struct SqlServerGuid : IComparable, IComparable<SqlServerGuid>, IEquatable<SqlServerGuid>, IFormattable
{
    private readonly Guid innerGuid;

    public SqlServerGuid(Guid guid)
    {
        innerGuid = guid;
    }

    public int CompareTo(SqlServerGuid value) => ((SqlGuid)innerGuid).CompareTo((SqlGuid)value.innerGuid);

    public int CompareTo(object? value) => ((SqlGuid)innerGuid).CompareTo(value);

    public bool Equals(SqlServerGuid g) => innerGuid.Equals(g.innerGuid);

    public override bool Equals(object? o) => innerGuid.Equals(o);
    
    public override int GetHashCode() => innerGuid.GetHashCode();

    public static SqlServerGuid NewGuid() => new(Guid.NewGuid());

    public static SqlServerGuid NewSequentialGuid(SequentialGuidOptions? options = default) =>
        new (SequentialGuidGenerator.Next(SequentialGuidType.SqlServer, options));

    public static bool operator ==(SqlServerGuid a, SqlServerGuid b) =>  a.innerGuid == b.innerGuid;

    public static bool operator !=(SqlServerGuid a, SqlServerGuid b) => a.innerGuid != b.innerGuid;

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static SqlServerGuid Parse(ReadOnlySpan<char> input) => new(Guid.Parse(input));
#endif

    public static SqlServerGuid Parse(string input) => new(Guid.Parse(input));

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static SqlServerGuid ParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format) => new(Guid.ParseExact(input, format));
#endif

    public static SqlServerGuid ParseExact(string input, string format) => new(Guid.ParseExact(input, format));

    public byte[] ToByteArray() => innerGuid.ToByteArray();

    public override string ToString() => innerGuid.ToString();

    public string ToString(string? format) => innerGuid.ToString(format);

    public string ToString(string? format, IFormatProvider? provider) => innerGuid.ToString(format, provider);

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default) =>
        innerGuid.TryFormat(destination, out charsWritten, format);
#endif

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParse(ReadOnlySpan<char> input, out SqlServerGuid result)
    {
        if(Guid.TryParse(input, out var guid))
        {
            result = new SqlServerGuid(guid);
            return true;
        }

        result = default;
        return false;
    }
#endif

#if NET48
    public static bool TryParse(string? input, out SqlServerGuid result)
#elif NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParse([NotNullWhen(true)] string? input, out SqlServerGuid result)
#endif
    {
        if (Guid.TryParse(input, out var guid))
        {
            result = new SqlServerGuid(guid);
            return true;
        }

        result = default;
        return false;
    }

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, out SqlServerGuid result)
    {
        if (Guid.TryParseExact(input, format, out var guid))
        {
            result = new SqlServerGuid(guid);
            return true;
        }

        result = default;
        return false;
    }
#endif

#if NET48
    public static bool TryParseExact(string? input, string? format, out SqlServerGuid result)
#elif NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public static bool TryParseExact([NotNullWhen(true)] string? input, [NotNullWhen(true)] string? format, out SqlServerGuid result)
#endif
    {
        if (Guid.TryParseExact(input, format, out var guid))
        {
            result = new SqlServerGuid(guid);
            return true;
        }

        result = default;
        return false;
    }

#if NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
    public bool TryWriteBytes(Span<byte> destination) => innerGuid.TryWriteBytes(destination);
#endif

    public static implicit operator Guid(SqlServerGuid x) => x.innerGuid;

    public static implicit operator SqlServerGuid(Guid x) => new(x);
}