using Astryx.Abstractions.Runtime;
using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Factories;

namespace Astryx.Client.TestHarness.TestSuite
{
    /// <summary>
    /// Diagnostic-grade, zero-allocation simulation wrapper.
    /// This version is safe for 100M–1B step endurance runs.
    /// </summary>
    public sealed class TestSimulation
    {
        private readonly IRuntimeService _runtime;
        private readonly List<IAgentActor> _actors = new();

        public TestSimulation(IRuntimeService runtime)
        {
            _runtime = runtime;
        }

        /// <summary>
        /// Registers an actor with both the simulation and the runtime.
        /// </summary>
        public void AddActor(IAgentActor actor)
        {
            _actors.Add(actor);
            _runtime.RegisterActor(actor);
        }

        /// <summary>
        /// Runs N steps with zero allocations and zero retention.
        /// StepSnapshot returned by runtime is discarded immediately.
        /// </summary>
        public void RunSteps(int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Streaming step: no retention, no allocations.
                _runtime.Step();
            }
        }
    }
}