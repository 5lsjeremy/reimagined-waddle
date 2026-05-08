using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Factories;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Sparks;

namespace Astryx.Client.TestHarness.TestSuite
{
    public sealed class HybridSparkEngineSet : ISparkEngineSet
    {
        private readonly ISparkEngine[] _lifecycleEngines;
        private readonly List<ISpark> _activeSparks = new();
        private readonly IAstryxMath _math;
        private readonly IAstryxVector _scratch;

        public HybridSparkEngineSet(
            IAstryxMath math,
            ISparkEngine[] lifecycleEngines)
        {
            _math = math;
            _lifecycleEngines = lifecycleEngines;
            _scratch = math.ZeroVector(3);
        }

        public void RegisterSpark(ISpark spark)
        {
            if (!_activeSparks.Contains(spark))
                _activeSparks.Add(spark);
        }

        public (IAstryxVector force, ActorState updatedState) Compute(AgentProfile profile, ActorState state,
            IAstryxMath math)
        {
            foreach (var engine in _lifecycleEngines)
                engine.Tick(0.016f);

            var sum = _math.ZeroVector(3);

            foreach (var spark in _activeSparks)
            {
                if (!spark.IsActive)
                    continue;

                // WOUND SPARK HANDLING
                if (spark.IsWound)
                {
                    if (state.Wound is null || !state.Wound.IsActive)
                    {
                        state = state with
                        {
                            Wound = new WoundState(0.8f, 200, true)
                        };

                        // Disable the spark permanently
                        spark.IsActive = false;
                    }
                }
                
                // NORMAL SPARK FORCE
                for (int i = 0; i < _scratch.Length; i++)
                    _scratch.Values[i] = 0f;

                var f = spark.Compute(_scratch, profile);

                for (int i = 0; i < sum.Length; i++)
                    sum.Values[i] += f.Values[i];
            }

            return (sum,state);
        }
    }
}
