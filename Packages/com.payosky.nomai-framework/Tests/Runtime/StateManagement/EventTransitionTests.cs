using System.Threading.Tasks;
using NomaiFramework.EventBus;
using NomaiFramework.Services;
using NomaiFramework.StateManagement;
using NomaiFramework.Tests.EventBus;
using NUnit.Framework;

namespace NomaiFramework.Tests.StateManagement
{
    public sealed class EventTransitionTests
    {
        private StateMachine _stateMachine;

        [SetUp]
        public async Task Setup()
        {
            await ServiceLocator.AddService(new EventBusService());
            _stateMachine = new StateMachine();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Dispose();
            _stateMachine?.Dispose();
        }

        [Test]
        public void EventTriggered_TransitionsToTargetState()
        {
            TestState targetState = new();
            EventListener<TestEvent> listener = new(_ => { });
            listener.Register();

            using EventTransition transition = new(_stateMachine, targetState, listener);

            listener.Trigger(new TestEvent());

            Assert.That(_stateMachine.CurrentState, Is.SameAs(targetState));
        }

        [Test]
        public void EventTriggered_TransitionsOnlyOnce()
        {
            TestState targetState = new();
            EventListener<TestEvent> listener = new(_ => { });
            listener.Register();

            using EventTransition transition = new(
                _stateMachine,
                targetState,
                listener
            );

            listener.Trigger(new TestEvent());
            listener.Trigger(new TestEvent());

            Assert.That(targetState.EnterCount, Is.EqualTo(1));
        }

        [Test]
        public void NullEventListener_DoesNotThrow()
        {
            TestState targetState = new();

            Assert.DoesNotThrow(() =>
            {
                using EventTransition transition = new(
                    _stateMachine,
                    targetState,
                    null
                );
            });
        }
    }
}