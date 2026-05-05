using Astryx.Abstractions;
using Astryx.Abstractions.Agents;
using Astryx.Abstractions.Runtime;
using Astryx.Substrate;
using Astryx.Substrate.Factories;
using Astryx.Substrate.Runtime;

namespace Astryx.Client.TestHarness
{
    public class ActorFactoryTests
    {
        [Test]
        public void Can_Create_Actor_From_Profile()
        {
            var identity = new Matrix(new float[,] 
            {
                { 1f, 0f },
                { 0f, 1f }
            });

            var resolver = new ProfileResolver();
            var factory = new AgentActorFactory(resolver);

            var actor = factory.Create(identity);
            actor.Profile.Name = "TestAgent";

            Assert.That(actor.Profile.Name, Is.EqualTo("TestAgent"));
        }

    }

    public class RuntimeTests
    {
        [Test]
        public void EndToEndRuntimeExecutesOneStep()
        {
            var identity = new Matrix(new float[,] 
            {
                { 1f, 0f },
                { 0f, 1f }
            });

            var resolver = new ProfileResolver();
            var factory = new AgentActorFactory(resolver);

            var actor = factory.Create(identity);
            actor.Profile.Name = "TestAgent";

            IRuntimeService runtime = new RuntimeService();
            runtime.RegisterActor(actor);

            runtime.Step();

            Assert.That(actor.Profile.Name, Is.EqualTo("TestAgent"));
        }
    }
}