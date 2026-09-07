using System.Collections.Generic;
using NUnit.Framework;
using NomaiFramework.EventBus;

namespace NomaiFramework.Tests.EventBus
{
    public class EventBusServiceTests
    {
        private EventBusService _eventBus;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new EventBusService();
        }

        [Test]
        public void Subscribe_WhenEventIsTriggered_TriggersListener()
        {
            TestEvent payload = new();
            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            _eventBus.Subscribe(listener);

            _eventBus.Trigger(payload);

            Assert.AreEqual(1, triggerCount);
        }

        [Test]
        public void Subscribe_WhenMultipleListenersHaveSamePayload_TriggersAllListeners()
        {
            TestEvent payload = new();

            int firstTriggerCount = 0;
            int secondTriggerCount = 0;

            EventListener<TestEvent> firstListener = new(_ => { firstTriggerCount++; });

            EventListener<TestEvent> secondListener = new(_ => { secondTriggerCount++; });

            _eventBus.Subscribe(firstListener);
            _eventBus.Subscribe(secondListener);

            _eventBus.Trigger(payload);

            Assert.AreEqual(1, firstTriggerCount);
            Assert.AreEqual(1, secondTriggerCount);
        }

        [Test]
        public void Subscribe_WhenListenerIsAlreadySubscribed_DoesNotSubscribeTwice()
        {
            TestEvent payload = new();
            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            _eventBus.Subscribe(listener);
            _eventBus.Subscribe(listener);

            _eventBus.Trigger(payload);

            Assert.AreEqual(1, triggerCount);
        }

        [Test]
        public void Subscribe_WhenListenerIsNull_DoesNothing()
        {
            Assert.DoesNotThrow(() => { _eventBus.Subscribe(null); });
        }

        [Test]
        public void Trigger_OnlyTriggersListenersForMatchingPayload()
        {
            int testEventTriggerCount = 0;
            int anotherEventTriggerCount = 0;

            EventListener<TestEvent> testEventListener = new(_ => { testEventTriggerCount++; });

            EventListener<AnotherTestEvent> anotherEventListener = new(_ => { anotherEventTriggerCount++; });

            _eventBus.Subscribe(testEventListener);
            _eventBus.Subscribe(anotherEventListener);

            _eventBus.Trigger(new TestEvent());

            Assert.AreEqual(1, testEventTriggerCount);
            Assert.AreEqual(0, anotherEventTriggerCount);
        }

        [Test]
        public void Trigger_PassesPayloadToListener()
        {
            TestEvent payload = new();

            TestEvent receivedPayload = null;

            EventListener<TestEvent> listener = new(received => { receivedPayload = received; });

            _eventBus.Subscribe(listener);

            _eventBus.Trigger(payload);

            Assert.AreSame(payload, receivedPayload);
        }

        [Test]
        public void Trigger_WhenPayloadIsNull_DoesNothing()
        {
            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            _eventBus.Subscribe(listener);

            Assert.DoesNotThrow(() => { _eventBus.Trigger<TestEvent>(null); });

            Assert.AreEqual(0, triggerCount);
        }

        [Test]
        public void Trigger_InvokesOnEventDispatched()
        {
            TestEvent payload = new();

            IEvent receivedPayload = null;

            _eventBus.OnEventDispatched += received => { receivedPayload = received; };

            _eventBus.Trigger(payload);

            Assert.AreSame(payload, receivedPayload);
        }

        [Test]
        public void Trigger_InvokesOnEventDispatchedBeforeListener()
        {
            int dispatchOrder = 0;
            int eventDispatchedOrder = 0;
            int listenerOrder = 0;

            EventListener<TestEvent> listener = new(_ => { listenerOrder = ++dispatchOrder; });

            _eventBus.OnEventDispatched += _ => { eventDispatchedOrder = ++dispatchOrder; };

            _eventBus.Subscribe(listener);

            _eventBus.Trigger(new TestEvent());

            Assert.AreEqual(1, eventDispatchedOrder);
            Assert.AreEqual(2, listenerOrder);
        }

        [Test]
        public void Trigger_WhenThereAreNoListeners_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => { _eventBus.Trigger(new TestEvent()); });
        }

        [Test]
        public void Trigger_PassesPayloadDataToListener()
        {
            TestEvent payload = new(42, "Hello World");

            int receivedValue = 0;
            string receivedMessage = null;

            EventListener<TestEvent> listener = new(received =>
            {
                receivedValue = received.Value;
                receivedMessage = received.Message;
            });

            _eventBus.Subscribe(listener);

            _eventBus.Trigger(payload);

            Assert.AreEqual(42, receivedValue);
            Assert.AreEqual("Hello World", receivedMessage);
        }

        [Test]
        public void Trigger_PassesPayloadDataToOnEventDispatched()
        {
            TestEvent payload = new(42, "Hello World");

            TestEvent receivedPayload = null;

            _eventBus.OnEventDispatched += received => { receivedPayload = received as TestEvent; };

            _eventBus.Trigger(payload);

            Assert.IsNotNull(receivedPayload);
            Assert.AreEqual(42, receivedPayload.Value);
            Assert.AreEqual("Hello World", receivedPayload.Message);
        }

        [Test]
        public void Trigger_WithDifferentPayloads_PassesCorrectDataEachTime()
        {
            List<TestEvent> receivedPayloads = new();

            EventListener<TestEvent> listener = new(receivedPayloads.Add);

            _eventBus.Subscribe(listener);

            _eventBus.Trigger(new TestEvent(1, "First"));

            _eventBus.Trigger(new TestEvent(2, "Second"));

            Assert.AreEqual(2, receivedPayloads.Count);

            Assert.AreEqual(1, receivedPayloads[0].Value);
            Assert.AreEqual("First", receivedPayloads[0].Message);

            Assert.AreEqual(2, receivedPayloads[1].Value);
            Assert.AreEqual("Second", receivedPayloads[1].Message);
        }

        [Test]
        public void Unsubscribe_PreventsListenerFromBeingTriggered()
        {
            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            _eventBus.Subscribe(listener);
            _eventBus.Unsubscribe(listener);

            _eventBus.Trigger(new TestEvent());

            Assert.AreEqual(0, triggerCount);
        }

        [Test]
        public void Unsubscribe_OnlyRemovesSpecifiedListener()
        {
            int firstTriggerCount = 0;
            int secondTriggerCount = 0;

            EventListener<TestEvent> firstListener = new(_ => { firstTriggerCount++; });

            EventListener<TestEvent> secondListener = new(_ => { secondTriggerCount++; });

            _eventBus.Subscribe(firstListener);
            _eventBus.Subscribe(secondListener);

            _eventBus.Unsubscribe(firstListener);

            _eventBus.Trigger(new TestEvent());

            Assert.AreEqual(0, firstTriggerCount);
            Assert.AreEqual(1, secondTriggerCount);
        }

        [Test]
        public void Unsubscribe_WhenListenerWasNotSubscribed_DoesNothing()
        {
            EventListener<TestEvent> listener = new(_ => { });

            Assert.DoesNotThrow(() => { _eventBus.Unsubscribe(listener); });
        }

        [Test]
        public void Unsubscribe_WhenListenerIsNull_DoesNothing()
        {
            Assert.DoesNotThrow(() => { _eventBus.Unsubscribe(null); });
        }

        [Test]
        public void Unsubscribe_WhenCalledMultipleTimes_DoesNotThrow()
        {
            EventListener<TestEvent> listener = new(_ => { });

            _eventBus.Subscribe(listener);

            _eventBus.Unsubscribe(listener);

            Assert.DoesNotThrow(() => { _eventBus.Unsubscribe(listener); });
        }

        [Test]
        public void Subscribe_AfterUnsubscribe_CanSubscribeAgain()
        {
            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            _eventBus.Subscribe(listener);
            _eventBus.Unsubscribe(listener);
            _eventBus.Subscribe(listener);

            _eventBus.Trigger(new TestEvent());

            Assert.AreEqual(1, triggerCount);
        }
    }
}