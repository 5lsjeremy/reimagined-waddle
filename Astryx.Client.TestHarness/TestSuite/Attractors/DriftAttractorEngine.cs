using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Runtime.Engines;

namespace Astryx.Client.TestHarness.TestSuite.Attractors
{
    public sealed class DriftAttractorEngine : IAttractorEngine
    {
        private readonly IAstryxMath _math;

        public DriftAttractorEngine(IAstryxMath math)
        {
            _math = math;
            Id = "drift-attractor";
        }

        public string Id { get; }

        public IAstryxVector Compute(AgentProfile profile, ActorState state, IAstryxMath math)
        {
            var v = math.ZeroVector(3);

            float drift = profile.Drift;

            v.Values[0] = -drift * 0.05f;
            v.Values[1] = -drift * 0.10f;
            v.Values[2] = drift * 0.03f;

            return v;
        }
    }
}