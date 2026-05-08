using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Runtime.Engines;

namespace Astryx.Client.TestHarness.TestSuite.Attractors
{
    public sealed class StabilityAttractorEngine : IAttractorEngine
    {
        private readonly IAstryxMath _math;

        public StabilityAttractorEngine(IAstryxMath math)
        {
            _math = math;
            Id = "stability-attractor";
        }

        public string Id { get; }

        public IAstryxVector Compute(AgentProfile profile, ActorState state, IAstryxMath math)
        {
            var v = math.ZeroVector(3);

            float stab = profile.IdentityStability;

            v.Values[0] = stab * 0.02f;
            v.Values[1] = -stab * 0.01f;
            v.Values[2] = stab * 0.04f;

            return v;
        }
    }
}