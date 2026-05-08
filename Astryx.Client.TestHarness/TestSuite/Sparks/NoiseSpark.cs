using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Math;

namespace Astryx.Client.TestHarness.TestSuite.Sparks
{
    public sealed class NoiseSpark : BaseSpark
    {
        public NoiseSpark(string id, float intensity)
            : base(id, intensity) { }

        public override IAstryxVector Compute(IAstryxVector state, AgentProfile profile)
        {
            for (int i = 0; i < state.Length; i++)
                state.Values[i] = 0f;

            float mag = Intensity * 0.05f;

            state.Values[0] = mag;
            state.Values[1] = -mag * 0.5f;
            state.Values[2] = mag * 0.25f;

            return state;
        }
    }
}