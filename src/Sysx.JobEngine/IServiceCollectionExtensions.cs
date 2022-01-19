﻿using Sysx.DependencyInjection;

namespace Sysx.JobEngine;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddAssetContext(
        this IServiceCollection services,
        Type assetContextType,
        IEnumerable<Type>? assetTypes = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        var isAssetContextType = typeof(AssetContext).IsAssignableFrom(assetContextType);

        if (!isAssetContextType)
            throw new InvalidOperationException($"Type {nameof(isAssetContextType)} {isAssetContextType} is not an AssetContext type.");

        services.Add(new ServiceDescriptor(assetContextType, services => services.Activate(assetContextType, assetTypes ?? Type.EmptyTypes), serviceLifetime));

        var ancestorContextType = assetContextType.BaseType;

        while (ancestorContextType != null)
        {
            if (!ancestorContextType.IsAbstract && ancestorContextType.IsPublic)
            {
                services.Add(new ServiceDescriptor(ancestorContextType, services => services.GetRequiredService(assetContextType), serviceLifetime));
            }

            ancestorContextType = ancestorContextType.BaseType;
        }

        return services;
    }

    public static IServiceCollection AddQueueServiceToQueue(
        this IServiceCollection services,
        Type queueType,
        string name = QueueLocator.DefaultQueueName,
        ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        var isQueueType = typeof(IQueue).IsAssignableFrom(queueType);
        
        if (!isQueueType)
            throw new InvalidOperationException($"Type {nameof(isQueueType)} {isQueueType} is not a Queue type.");

        services.Add(new ServiceDescriptor(
            queueType,
            serviceProvider => serviceProvider.GetRequiredService<IQueueLocator>().Get(queueType, name),
            serviceLifetime));

        return services;
    }

    public static IServiceCollection AddJobExecutor(
        this IServiceCollection services,
        Type jobExecutorType,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        var isJobExecutor = jobExecutorType.IsAssignableToGenericType(typeof(IJobExecutor<>));

        if (!isJobExecutor)
            throw new InvalidOperationException($"Type {nameof(jobExecutorType)} {jobExecutorType} is not a JobExecutor type.");

        services.AddClosedType(typeof(IJobExecutor<>), jobExecutorType, serviceLifetime);

        return services;
    }

    [Obsolete("This API may be removed. Prefer IOnJobExecuteEvent instead.")]
    public static IServiceCollection AddOnAssetEvent(
        this IServiceCollection services,
        Type onAssetEventType,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        var isOnGetAssetEvent = onAssetEventType.IsAssignableToGenericType(typeof(IOnGetAssetEvent<,>));
        var isOnAddAssetEvent = onAssetEventType.IsAssignableToGenericType(typeof(IOnAddAssetEvent<,>));
        var isOnUpsertAssetEvent = onAssetEventType.IsAssignableToGenericType(typeof(IOnUpsertAssetEvent<,>));
        var isOnUpdateAssetEvent = onAssetEventType.IsAssignableToGenericType(typeof(IOnUpdateAssetEvent<,>));
        var isOnDeleteAssetEvent = onAssetEventType.IsAssignableToGenericType(typeof(IOnDeleteAssetEvent<,>));

        if (!isOnGetAssetEvent
            && !isOnAddAssetEvent
            && !isOnUpsertAssetEvent
            && !isOnUpdateAssetEvent
            && !isOnDeleteAssetEvent)
            throw new InvalidOperationException($"Type {nameof(onAssetEventType)} {onAssetEventType} is not an OnAssetEvent type.");

        if (isOnGetAssetEvent) services.AddOpenOrClosedType(typeof(IOnGetAssetEvent<,>), onAssetEventType, serviceLifetime);

        if (isOnAddAssetEvent) services.AddOpenOrClosedType(typeof(IOnAddAssetEvent<,>), onAssetEventType, serviceLifetime);

        if (isOnUpsertAssetEvent) services.AddOpenOrClosedType(typeof(IOnUpsertAssetEvent<,>), onAssetEventType, serviceLifetime);

        if (isOnUpdateAssetEvent) services.AddOpenOrClosedType(typeof(IOnUpdateAssetEvent<,>), onAssetEventType, serviceLifetime);

        if (isOnDeleteAssetEvent) services.AddOpenOrClosedType(typeof(IOnDeleteAssetEvent<,>), onAssetEventType, serviceLifetime);

        return services;
    }

    public static IServiceCollection AddOnJobExecute(
        this IServiceCollection services,
        Type onJobExecuteType,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
    {
        var isOnJobExecute = onJobExecuteType.IsAssignableToGenericType(typeof(IOnJobExecuteEvent<,>));

        if (!isOnJobExecute)
            throw new InvalidOperationException($"Type {nameof(onJobExecuteType)} {onJobExecuteType} is not an OnJobExecute type.");

        services.AddOpenOrClosedType(typeof(IOnJobExecuteEvent<,>), onJobExecuteType, serviceLifetime);

        return services;
    }

    private static IServiceCollection AddClosedType(
        this IServiceCollection services,
        Type openServiceType,
        Type implementationType,
        ServiceLifetime serviceLifetime)
    {
        var implementedOpenServiceType = implementationType.GetGenericTypeImplementation(openServiceType);

        EnsureArg.IsNotNull(
            implementedOpenServiceType,
            optsFn: x => x.WithMessage($"Type {implementationType} must implemenet {openServiceType}."));
        EnsureArg.IsFalse(
            implementedOpenServiceType.ContainsGenericParameters,
            optsFn: x => x.WithMessage($"Type {implementationType} is an open type and must be closed."));

        if (implementedOpenServiceType.ContainsGenericParameters)
        {
            services.Add(new ServiceDescriptor(openServiceType, implementationType, serviceLifetime));
        }
        else
        {
            services.Add(new ServiceDescriptor(implementedOpenServiceType, implementationType, serviceLifetime));
        }

        return services;
    }

    private static IServiceCollection AddOpenOrClosedType(
        this IServiceCollection services,
        Type openServiceType,
        Type implementationType,
        ServiceLifetime serviceLifetime)
    {
        var implementedOpenServiceType = implementationType.GetGenericTypeImplementation(openServiceType);

        EnsureArg.IsNotNull(implementedOpenServiceType, optsFn: x => x.WithMessage($"Type {implementationType} must implemenet {openServiceType}."));

        if (implementedOpenServiceType.ContainsGenericParameters)
        {
            services.Add(new ServiceDescriptor(openServiceType, implementationType, serviceLifetime));
        }
        else
        {
            services.Add(new ServiceDescriptor(implementedOpenServiceType, implementationType, serviceLifetime));
        }

        return services;
    }
}
