﻿namespace Sysx.EntityFramework.Sqlite.RelationalTypeMappings;

public class BinaryGuidTypeMapping : RelationalTypeMapping
{
    private static readonly ValueConverter<BinaryGuid, byte[]> converter
       = new(x => x.ToByteArray(), x => new BinaryGuid(new Guid(x)));

    public override Type ClrType => typeof(BinaryGuid);
    public override ValueConverter Converter => converter;
    public override string StoreType => "BLOB";
    public override DbType? DbType => System.Data.DbType.Binary;
    public override int? Size => 16;
    public override bool IsFixedLength => true;

    public BinaryGuidTypeMapping() : base(new RelationalTypeMappingParameters()) { }

    protected override RelationalTypeMapping Clone(RelationalTypeMappingParameters parameters) => new BinaryGuidTypeMapping();

    protected override string GenerateNonNullSqlLiteral(object value)
    {
        EnsureArg.IsNotNull(value, nameof(value));

        var bytes = (byte[])value;

        var hex = new StringBuilder("0x", (bytes.Length * 2) + 2);

        foreach (byte b in bytes)
            hex.AppendFormat("{0:x2}", b);

        return hex.ToString();
    }
}