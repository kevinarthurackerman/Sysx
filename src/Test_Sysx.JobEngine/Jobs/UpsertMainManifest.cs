﻿namespace Test_Sysx.JobEngine.Jobs;

public static class UpsertMainManifest
{
    public readonly record struct Job : IJob { }

    public class Handler : IJobExecutor<Job>
    {
        private readonly AssetContext assetContext;

        public Handler(AssetContext assetContext)
        {
            this.assetContext = assetContext;
        }

        public void Execute(in Job data)
        {
            if (!assetContext.Manifests().TryGet("main", out var _))
            {
                assetContext.Manifests().Add(new Manifest("main", Array.Empty<Guid>()));
            }
        }
    }
}