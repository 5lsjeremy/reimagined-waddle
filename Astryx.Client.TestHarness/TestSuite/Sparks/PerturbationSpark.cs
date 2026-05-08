using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Math;

namespace Astryx.Client.TestHarness.TestSuite.Sparks
{
    public sealed class PerturbationSpark : BaseSpark
    {
        public PerturbationSpark(string id, float intensity)
            : base(id, intensity) { }

        public override IAstryxVector Compute(IAstryxVector state, AgentProfile profile)
        {
            for (int i = 0; i < state.Length; i++)
                state.Values[i] = 0f;

            float drift = profile.Drift;
            float mag = Intensity * (0.5f + MathF.Abs(drift));

            state.Values[0] = 0f;
            state.Values[1] = mag;
            state.Values[2] = -mag;

            return state;
        }
    }
}