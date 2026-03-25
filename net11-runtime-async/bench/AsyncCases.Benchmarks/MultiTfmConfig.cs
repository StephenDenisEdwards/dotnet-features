using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace AsyncCases.Benchmarks;

public class MultiTfmConfig : ManualConfig
{
    public MultiTfmConfig()
    {
        AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0));
        AddJob(Job.Default.WithRuntime(CoreRuntime.Core11_0)
            .WithMsBuildArguments("/p:Features=runtime-async=on", "/p:EnablePreviewFeatures=true"));
    }
}
