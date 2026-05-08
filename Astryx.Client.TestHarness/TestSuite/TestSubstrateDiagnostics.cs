using System;
using System.Linq;
using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Factories;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Oracle;
using Astryx.Abstractions.Runtime;
using Astryx.Abstractions.Runtime.Engines;
using Astryx.Abstractions.Sparks;
using Astryx.Abstractions.Trauma;
using Astryx.Client.TestHarness.TestSuite.Attractors;
using Astryx.Client.TestHarness.TestSuite.Pipeline;
using Astryx.Client.TestHarness.TestSuite.Sparks;
using Astryx.Oracles.Types;
using Astryx.Runtime.Service;
using Astryx.Substrate.AstryxMath;
using Astryx.Substrate.AstryxMath.Services;
using Astryx.Substrate.Eigen;
using Astryx.Substrate.Factories;
using Astryx.Substrate.Agents;
using Astryx.Substrate.Trauma;

namespace Astryx.Client.TestHarness.TestSuite
{
    public static class TestSubstrateDiagnostics
    {
        public static void Run()
        {
            Console.WriteLine("=== ASTRYX SUBSTRATE DIAGNOSTIC (BEHAVIORAL + TRAUMA) ===");

            IAstryxMath math = new SubstrateMathService();
            IEigenSolver eigen = new SubstrateEigenSolver();

            // ---------------------------------------------------------
            // TRAUMA RESOLVERS
            // ---------------------------------------------------------
            var traumaThresholds = new TraumaThresholds();
            var traumaResolver = new TraumaStateResolver(traumaThresholds);
            var traumaBehaviorResolver = new TraumaBehaviorResolver();

            // Profile resolver now receives trauma resolver
            IProfileResolver resolver = new ProfileResolver(math, eigen, traumaResolver);

            // ---------------------------------------------------------
            // ATTRACTOR ENGINE SET (raw engines)
            // ---------------------------------------------------------
            var attractors = new AttractorEngineSet(new IAttractorEngine[]
            {
                new BasicAttractorEngine(math),
                new DriftAttractorEngine(math),
                new StabilityAttractorEngine(math)
            });

            // ---------------------------------------------------------
            // SPARK ENGINE SET (raw)
            // ---------------------------------------------------------
            var hybridSparks = new HybridSparkEngineSet(math, Array.Empty<ISparkEngine>());
            hybridSparks.RegisterSpark(new WoundSpark("wound-1", 0.8f, lifecycleSteps: 200));

            ISparkEngineSet sparks = hybridSparks;

            // ---------------------------------------------------------
            // UNIFIED TRAUMA-AWARE DECORATOR
            // ---------------------------------------------------------
            var engines = new EngineSetDecorator(attractors, sparks, traumaBehaviorResolver);

            // ---------------------------------------------------------
            // ORACLE + PIPELINE + RUNTIME
            // ---------------------------------------------------------
            IOracleSignal oracle = new OracleSignal(
                sourceId: "diagnostic",
                type: "full",
                tags: new[] { "cognitive", "evolution", "diagnostic" },
                metadata: null
            );

            IRuntimePipeline pipeline = new CognitivePipeline(math);
            var runtime = new RuntimeService(pipeline, math);
            var sim = new TestSimulation(runtime);

            // ---------------------------------------------------------
            // ACTOR FACTORY (uses unified engine set for both)
            // ---------------------------------------------------------
            var factory = new AgentActorFactory(resolver, engines, engines, oracle, math);

            var identity = new Matrix(new float[,]
            {
                { 1f, 0f },
                { 0f, 1f }
            });

            var actor = factory.Create(identity);
            actor.Profile.Name = "DiagnosticActor";

            sim.AddActor(actor);

            // ---------------------------------------------------------
            // DIAGNOSTIC RUNS
            // ---------------------------------------------------------
            RunTimed(sim, 10, "Baseline (10 steps)", actor);
            RunTimed(sim, 1, "Short Horizon (1 step)", actor);
            RunTimed(sim, 5, "Medium Horizon (5 steps)", actor);
            RunTimed(sim, 10, "Extended (10 steps)", actor);

            RunTimed(sim, 100, "Long Horizon (100 steps)", actor);
            RunTimed(sim, 1_000, "Long Horizon (1,000 steps)", actor);
            RunTimed(sim, 10_000, "Long Horizon (10,000 steps)", actor);
            RunTimed(sim, 100_000, "Long Horizon (100,000 steps)", actor);

            Console.WriteLine("\n=== DIAGNOSTIC COMPLETE ===");
        }

        private static void RunTimed(TestSimulation sim, int steps, string label, IAgentActor actor)
        {
            Console.WriteLine($"\n--- {label} ---");

            var sw = System.Diagnostics.Stopwatch.StartNew();
            sim.RunSteps(steps);
            sw.Stop();

            double seconds = sw.Elapsed.TotalSeconds;
            double tps = steps / seconds;

            Console.WriteLine($"[Timing] {steps:N0} steps in {seconds:F4}s ({tps:N0} steps/sec)");

            PrintProfile(actor.Profile, $"After +{steps:N0} steps");
            PrintState(actor.State);
        }

        private static void PrintProfile(AgentProfile profile, string label)
        {
            Console.WriteLine($"\n{label}");
            Console.WriteLine($"  Name:              {profile.Name}");
            Console.WriteLine($"  Alignment:         {profile.Alignment:F4}");
            Console.WriteLine($"  Momentum:          {profile.Momentum:F4}");
            Console.WriteLine($"  Drift:             {profile.Drift:F4}");
            Console.WriteLine($"  Direction:         [{string.Join(", ", profile.Direction.Values.Select(v => v.ToString("F3")))}]");
            Console.WriteLine($"  AlignmentVector:   [{string.Join(", ", profile.AlignmentVector.Values.Select(v => v.ToString("F3")))}]");
            Console.WriteLine($"  IdentityStability: {profile.IdentityStability:F4}");
            Console.WriteLine($"  EmotionalStability:{profile.EmotionalStability:F4}");
            Console.WriteLine($"  SparkPotential:    {profile.SparkPotential:F4}");
            Console.WriteLine($"  SparkSensitivity:  {profile.SparkSensitivity:F4}");
            Console.WriteLine($"  SparkVector:       [{string.Join(", ", profile.SparkVector.Values.Select(v => v.ToString("F3")))}]");

            // ---------------------------------------------------------
            // TRAUMA DIAGNOSTICS
            // ---------------------------------------------------------
            Console.WriteLine($"  ScarLoad:          {profile.ScarLoad:F6}");
            Console.WriteLine($"  ScarCount:         {profile.ScarCount}");
            Console.WriteLine($"  TraumaState:       {profile.TraumaState}");

            // Derived trauma metrics
            Console.WriteLine($"  ScarImpact:        {profile.ScarImpact:F6}");
        }

        private static void PrintState(ActorState state)
        {
            Console.WriteLine("  --- ActorState ---");
            Console.WriteLine($"  State.Direction:       [{string.Join(", ", state.Direction.Values.Select(v => v.ToString("F3")))}]");
            Console.WriteLine($"  State.Momentum:        {state.Momentum:F4}");
            Console.WriteLine($"  State.Drift:           {state.Drift:F4}");
            Console.WriteLine($"  State.Alignment:       {state.Alignment:F4}");
            Console.WriteLine($"  State.AlignmentVector: [{string.Join(", ", state.AlignmentVector.Values.Select(v => v.ToString("F3")))}]");
        }
    }
}
