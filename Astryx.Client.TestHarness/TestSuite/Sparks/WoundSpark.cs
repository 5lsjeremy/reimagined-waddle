using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Math;

namespace Astryx.Client.TestHarness.TestSuite.Sparks
{
    public sealed class WoundSpark : BaseSpark
    {
        private readonly int _lifecycle;
        private readonly float _threshold;

        public WoundSpark(
            string id,
            float intensity,
            int lifecycleSteps = 300,
            float crystallizationThreshold = 0.05f)
            : base(id, intensity)
        {
            _lifecycle = lifecycleSteps;
            _threshold = crystallizationThreshold;
        }

        public override bool IsWound => true;
        public override int LifecycleSteps => _lifecycle;
        public override float CrystallizationThreshold => _threshold;

        public override IAstryxVector Compute(IAstryxVector state, AgentProfile profile)
        {
            // Wound sparks do NOT produce force.
            for (int i = 0; i < state.Length; i++)
                state.Values[i] = 0f;

            return state;
        }
    }
}