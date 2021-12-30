﻿namespace Sysx.EntityFramework.SqlServer.NodaTime.Plugins;

public class NodaTimeDbContextOptionsExtension : BaseContainerTypesDbContextOptionsExtension
{
    public NodaTimeDbContextOptionsExtension() : base("NodaTime") { }

    public override void ApplyServices(IServiceCollection services)
    {
        EnsureArg.IsNotNull(services, nameof(services));

        base.ApplyServices(services);

        services.UpsertScoped<RelationalTypeMapping, DurationTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, InstantTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, LocalDateTimeTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, LocalDateTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, LocalTimeTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, OffsetDateTimeTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, OffsetTypeMapping>();
        services.UpsertScoped<RelationalTypeMapping, ZonedDateTimeTypeMapping>();
    }
}