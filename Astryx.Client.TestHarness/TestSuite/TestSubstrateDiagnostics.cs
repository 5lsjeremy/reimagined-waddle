using Astryx.Abstractions;
using Astryx.Abstractions.Sparks;
using Astryx.Substrate.Factories;
using Astryx.Client.TestHarness.TestSuite;
using Astryx.Substrate.Runtime.Service;

namespace Astryx.Client.TestHarness.Diagnostics;

public static class TestSubstrateDiagnostics
{
    public static void Run()
    {
        Console.WriteLine("=== ASTRYX SUBSTRATE DIAGNOSTIC ===");

        // ---------------------------------------------------------
        // 0. Construct runtime + sim + resolver + factory + identity
        // ---------------------------------------------------------
        var runtime = new RuntimeService();
        var sim = new TestSimulation(runtime);

        var resolver = new ProfileResolver();
        var factory = new AgentActorFactory(resolver);

        var identity = new Matrix(new float[,]
        {
            { 1f, 0f },
            { 0f, 1f }
        });

        var actor = factory.Create(identity);
        actor.Profile.Name = "DiagnosticActor";

        sim.AddActor(actor);

        // ---------------------------------------------------------
        // TEST 1 — BASELINE STABILITY
        // ---------------------------------------------------------
        Console.WriteLine("\n--- TEST 1: Baseline Stability (10 steps, no sparks) ---");

        sim.RunSteps(10);
        PrintSnapshots(sim, 10);

        // ---------------------------------------------------------
        // TEST 2 — SINGLE SPARK
        // ---------------------------------------------------------
        Console.WriteLine("\n--- TEST 2: Single Spark Influence ---");

        sim.InjectSpark(new SparkEvent
        {
            SourceActorId = actor.Profile.Id,
            TargetActorId = actor.Profile.Id,
            Type = "overt",
            Magnitude = 1f
        });

        sim.RunSteps(1);
        PrintSnapshots(sim, 1);

        // ---------------------------------------------------------
        // TEST 3 — REPEATED SPARKS
        // ---------------------------------------------------------
        Console.WriteLine("\n--- TEST 3: Repeated Sparks (5 steps) ---");

        for (int i = 0; i < 5; i++)
        {
            sim.InjectSpark(new SparkEvent
            {
                SourceActorId = actor.Profile.Id,
                TargetActorId = actor.Profile.Id,
                Type = "latent",
                Magnitude = 0.5f
            });
        }

        sim.RunSteps(5);
        PrintSnapshots(sim, 5);

        // ---------------------------------------------------------
        // TEST 4 — RECOVERY
        // ---------------------------------------------------------
        Console.WriteLine("\n--- TEST 4: Recovery (10 steps, no sparks) ---");

        sim.RunSteps(10);
        PrintSnapshots(sim, 10);

        // ---------------------------------------------------------
        // TEST 5+ — LONG HORIZON STABILITY
        // ---------------------------------------------------------
        RunLongHorizon(sim, 100, "100");
        RunLongHorizon(sim, 1_000, "1,000");
        RunLongHorizon(sim, 10_000, "10,000");
        RunLongHorizon(sim, 100_000, "100,000");

        // Uncomment when ready for the big one:
        RunLongHorizon(sim, 1_000_000, "1,000,000");

        Console.WriteLine("\n=== DIAGNOSTIC COMPLETE ===");
    }

    private static void RunLongHorizon(TestSimulation sim, int steps, string label)
    {
        Console.WriteLine($"\n--- LONG HORIZON: {label} steps (no sparks) ---");

        int before = sim.SnapshotCount;
        sim.RunSteps(steps);
        int after = sim.SnapshotCount;

        var snap = sim.Snapshot(after - 1);
        var actor = snap.Actors.Values.First();

        Console.WriteLine($"\nFinal Step {snap.StepNumber}");
        Console.WriteLine($"  Alignment:       {actor.Alignment:F4}");
        Console.WriteLine($"  Momentum:        {actor.Momentum:F4}");
        Console.WriteLine($"  Drift:           {actor.Drift:F4}");
        Console.WriteLine($"  Stability:       {actor.Stability:F4}");
        Console.WriteLine($"  Direction:       [{string.Join(", ", actor.Direction.Values.Select(v => v.ToString("F3")))}]");
        Console.WriteLine($"  AlignmentVector: [{string.Join(", ", actor.AlignmentVector.Values.Select(v => v.ToString("F3")))}]");
    }

    private static void PrintSnapshots(TestSimulation sim, int count)
    {
        int start = sim.SnapshotCount - count;

        for (int i = start; i < sim.SnapshotCount; i++)
        {
            var snap = sim.Snapshot(i);
            var actor = snap.Actors.Values.First();

            Console.WriteLine($"\nStep {snap.StepNumber}");
            Console.WriteLine($"  Alignment:       {actor.Alignment:F4}");
            Console.WriteLine($"  Momentum:        {actor.Momentum:F4}");
            Console.WriteLine($"  Drift:           {actor.Drift:F4}");
            Console.WriteLine($"  Stability:       {actor.Stability:F4}");
            Console.WriteLine($"  Direction:       [{string.Join(", ", actor.Direction.Values.Select(v => v.ToString("F3")))}]");
            Console.WriteLine($"  AlignmentVector: [{string.Join(", ", actor.AlignmentVector.Values.Select(v => v.ToString("F3")))}]");

            if (snap.SparksConsumed.Count > 0)
            {
                Console.WriteLine("  Sparks Consumed:");
                foreach (var s in snap.SparksConsumed)
                    Console.WriteLine($"    - {s.Type} (mag {s.Magnitude})");
            }

            if (snap.Provenance.Count > 0)
            {
                Console.WriteLine("  Provenance:");
                foreach (var p in snap.Provenance)
                    Console.WriteLine($"    - [{p.Type}] {p.Source}: {p.Explanation}");
            }
        }
    }
}
