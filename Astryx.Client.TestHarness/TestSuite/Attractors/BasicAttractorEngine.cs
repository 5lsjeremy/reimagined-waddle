using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Runtime.Engines;

namespace Astryx.Client.TestHarness.TestSuite.Attractors
{
    public sealed class BasicAttractorEngine : IAttractorEngine
    {
        private readonly IAstryxMath _math;

        public BasicAttractorEngine(IAstryxMath math)
        {
            _math = math;
            Id = "basic-attractor";
        }

        public string Id { get; }

        public IAstryxVector Compute(AgentProfile profile, ActorState state, IAstryxMath math)
        {
            var v = math.ZeroVector(3);

            float a = profile.Alignment;

            v.Values[0] = a * 0.10f;
            v.Values[1] = a * 0.05f;
            v.Values[2] = -a * 0.02f;

            return v;
        }
    }
}