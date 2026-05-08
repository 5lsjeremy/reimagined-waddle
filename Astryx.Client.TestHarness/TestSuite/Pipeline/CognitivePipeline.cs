using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Factories;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Runtime.Engines;
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

            var attractorForce = actor.AttractorEngines.Compute(profile, state, _math);

            var (sparkForce, updatedState) = actor.SparkEngines.Compute(profile, state, _math);

            actor.State = updatedState;

            var output = Step(
                updatedState,
                profile,
                attractorForce,
                (sparkForce, updatedState),
                actor.OracleSignal,
                _math
            );

            actor.ApplyOutput(output, _math);
        }

        public IOutputVector Step(
            ActorState actorState,
            AgentProfile profile,
            IAstryxVector attractorForce,
            (IAstryxVector force, ActorState updatedState) sparkForce,
            IOracleSignal oracleSignal,
            IAstryxMath math)
        {
            var combined = math.ZeroVector(3);

            for (int i = 0; i < combined.Length; i++)
                combined.Values[i] = attractorForce.Values[i] + sparkForce.force.Values[i];

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