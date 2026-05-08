using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Factories;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Oracle;
using Astryx.Abstractions.Runtime;
using Astryx.Runtime.Engine;

namespace Astryx.Client.TestHarness.TestSuite.Pipeline
{
    public sealed class CognitivePipeline : IRuntimePipeline
    {
        private readonly IAstryxMath _math;

        public CognitivePipeline(IAstryxMath math)
        {
            _math = math;
        }

        public void Tick(IAgentActor actor, float deltaTime)
        {
            var profile = actor.Profile;
            var state   = actor.State;

            // 1. Compute forces from engine sets on the actor
            var attractorForce = actor.AttractorEngines.Compute(profile, state, _math);
            var sparkForce     = actor.SparkEngines.Compute(profile, state, _math);

            // 2. Produce output vector via Step
            var output = Step(state, profile, attractorForce, sparkForce, actor.OracleSignal, _math);

            // 3. Let the actor apply the output (per IAgentActor)
            actor.ApplyOutput(output, _math);
        }

        public IOutputVector Step(
            ActorState actorState,
            AgentProfile profile,
            IAstryxVector attractorForce,
            IAstryxVector sparkForce,
            IOracleSignal oracleSignal,
            IAstryxMath math)
        {
            // Combine forces into a single output vector
            var combined = math.ZeroVector(3);

            for (int i = 0; i < combined.Length; i++)
                combined.Values[i] = attractorForce.Values[i] + sparkForce.Values[i];

            return new OutputVector(combined);
        }
    }
    
    public sealed class OutputVector : IOutputVector
    {
        public IAstryxVector Value { get; }

        public OutputVector(IAstryxVector value)
        {
            Value = value;
        }
    }
}