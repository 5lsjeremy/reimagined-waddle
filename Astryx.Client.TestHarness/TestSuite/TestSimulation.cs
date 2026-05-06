using Astryx.Abstractions.Factories;
using Astryx.Abstractions.Runtime;
using Astryx.Abstractions.Snapshots;
using Astryx.Abstractions.Sparks;

namespace Astryx.Client.TestHarness.TestSuite;

public sealed class TestSimulation
{
    private readonly IRuntimeService _runtime;
    private readonly List<StepSnapshot> _snapshots = new();

    public TestSimulation(IRuntimeService runtime)
    {
        _runtime = runtime;
    }

    public void AddActor(IAgentActor actor)
    {
        _runtime.RegisterActor(actor);
    }

    public void InjectSpark(SparkEvent spark)
    {
        _runtime.InjectSpark(spark);
    }

    public void RunSteps(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var snapshot = _runtime.Step();
            _snapshots.Add(snapshot);
        }
    }

    public StepSnapshot Snapshot(int index) => _snapshots[index];
    public int SnapshotCount => _snapshots.Count;

}