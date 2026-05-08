using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Factories;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Runtime.Engines;

namespace Astryx.Client.TestHarness.TestSuite
{
    public sealed class AttractorEngineSet : IAttractorEngineSet
    {
        private readonly IAttractorEngine[] _engines;

        public AttractorEngineSet(IAttractorEngine[] engines)
        {
            _engines = engines;
        }

        public IAstryxVector Compute(AgentProfile profile, ActorState state, IAstryxMath math)
        {
            var sum = math.ZeroVector(3);

            foreach (var engine in _engines)
            {
                var f = engine.Compute(profile, state, math);
                for (int i = 0; i < sum.Length; i++)
                    sum.Values[i] += f.Values[i];
            }

            return sum;
        }
    }
}