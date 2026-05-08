using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Math;
using Astryx.Abstractions.Sparks;

namespace Astryx.Client.TestHarness.TestSuite.Sparks
{
    public abstract class BaseSpark : ISpark
    {
        public string Id { get; }
        public float Intensity { get; protected set; }
        public bool IsActive { get; set; }

        protected BaseSpark(string id, float intensity)
        {
            Id = id;
            Intensity = intensity;
            IsActive = true;
        }

        public abstract IAstryxVector Compute(IAstryxVector state, AgentProfile profile);
    }
}
