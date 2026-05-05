using Astryx.Abstractions;
using Astryx.Abstractions.Sparks;
using Astryx.Substrate.Factories;
using Astryx.Client.TestHarness.TestSuite;

namespace Astryx.Client.TestHarness
{
    public class ActorFactoryTests
    {
        [Test]
        public void Factory_Assigns_Id_And_Creates_Actor()
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

            Assert.That(actor.Profile.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(actor.Profile.Name, Is.EqualTo("TestAgent"));
        }
    }

    public class RuntimeTests
    {
        [Test]
        public void TestSimulation_Registers_Actor_And_Runs_Steps()
        {
            var runtime = new RuntimeService();
            var sim = new TestSimulation(runtime);

            var resolver = new ProfileResolver();
            var factory = new AgentActorFactory(resolver);

            var actor = factory.Create(new Matrix(new float[,] { { 1, 0 }, { 0, 1 } }));
            actor.Profile.Name = "TestAgent";

            sim.AddActor(actor);
            sim.RunSteps(1);

            var snapshot = sim.Snapshot(0);

            Assert.That(snapshot.Actors.ContainsKey(actor.Profile.Id), Is.True);
            Assert.That(snapshot.Actors[actor.Profile.Id].Name, Is.EqualTo("TestAgent"));
        }

        [Test]
        public void TestSimulation_Captures_Multiple_Snapshots()
        {
            var runtime = new RuntimeService();
            var sim = new TestSimulation(runtime);

            var resolver = new ProfileResolver();
            var factory = new AgentActorFactory(resolver);

            var actor = factory.Create(new Matrix(new float[,] { { 1, 0 }, { 0, 1 } }));
            sim.AddActor(actor);

            sim.RunSteps(3);

            Assert.That(sim.Snapshot(0).StepNumber, Is.EqualTo(1));
            Assert.That(sim.Snapshot(1).StepNumber, Is.EqualTo(2));
            Assert.That(sim.Snapshot(2).StepNumber, Is.EqualTo(3));
        }

        [Test]
        public void TestSimulation_Routes_Sparks_Through_Runtime()
        {
            var runtime = new RuntimeService();
            var sim = new TestSimulation(runtime);

            var resolver = new ProfileResolver();
            var factory = new AgentActorFactory(resolver);

            var actor = factory.Create(new Matrix(new float[,] { { 1, 0 }, { 0, 1 } }));
            sim.AddActor(actor);

            var spark = new SparkEvent
            {
                SourceActorId = actor.Profile.Id,
                TargetActorId = actor.Profile.Id,
                Type = "test",
                Magnitude = 1f
            };

            sim.InjectSpark(spark);

            Assert.DoesNotThrow(() => sim.RunSteps(1));
        }

        [Test]
        public void TestSimulation_Exposes_Snapshot_By_Index()
        {
            var runtime = new RuntimeService();
            var sim = new TestSimulation(runtime);

            var resolver = new ProfileResolver();
            var factory = new AgentActorFactory(resolver);

            var actor = factory.Create(new Matrix(new float[,] { { 1, 0 }, { 0, 1 } }));
            sim.AddActor(actor);

            sim.RunSteps(2);

            var snap1 = sim.Snapshot(0);
            var snap2 = sim.Snapshot(1);

            Assert.That(snap1.StepNumber, Is.EqualTo(1));
            Assert.That(snap2.StepNumber, Is.EqualTo(2));
        }
    }
}
