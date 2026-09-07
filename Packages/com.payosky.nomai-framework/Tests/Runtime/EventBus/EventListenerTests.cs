using System.Threading.Tasks;
using NomaiFramework.EventBus;
using NomaiFramework.Services;
using NUnit.Framework;

namespace NomaiFramework.Tests.EventBus
{
    public class EventListenerTests
    {
        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Dispose();
        }

        [Test]
        public void PayloadType_ReturnsGenericPayloadType()
        {
            EventListener<TestEvent> listener = new(_ => { });

            Assert.AreEqual(typeof(TestEvent), listener.PayloadType);
        }

        [Test]
        public void Trigger_WithMatchingPayload_InvokesCallback()
        {
            TestEvent payload = new();

            int callbackCount = 0;

            EventListener<TestEvent> listener = new(_ => { callbackCount++; });

            listener.Trigger(payload);

            Assert.AreEqual(1, callbackCount);
        }

        [Test]
        public void Trigger_WithMatchingPayload_PassesPayloadToCallback()
        {
            TestEvent payload = new();
            TestEvent receivedPayload = null;

            EventListener<TestEvent> listener = new(received => { receivedPayload = received; });

            listener.Trigger(payload);

            Assert.AreSame(payload, receivedPayload);
        }

        [Test]
        public void Trigger_WithMatchingPayload_SetsPayloadProperty()
        {
            TestEvent payload = new();

            EventListener<TestEvent> listener = new(_ => { });

            listener.Trigger(payload);

            Assert.AreSame(payload, listener.Payload);
        }

        [Test]
        public void Trigger_MultipleTimes_StoresLatestPayload()
        {
            TestEvent firstPayload = new();
            TestEvent secondPayload = new();

            EventListener<TestEvent> listener = new(_ => { });

            listener.Trigger(firstPayload);
            listener.Trigger(secondPayload);

            Assert.AreSame(secondPayload, listener.Payload);
        }

        [Test]
        public void Trigger_WithNullCallback_DoesNotThrow()
        {
            EventListener<TestEvent> listener = new(null);

            Assert.DoesNotThrow(() => { listener.Trigger(new TestEvent()); });
        }

        [Test]
        public void Trigger_WithWrongPayload_DoesNotInvokeCallback()
        {
            int callbackCount = 0;

            EventListener<TestEvent> listener = new(_ => { callbackCount++; });

            listener.Trigger(new AnotherTestEvent());

            Assert.AreEqual(0, callbackCount);
        }

        [Test]
        public void Trigger_WithWrongPayload_DoesNotChangePayload()
        {
            TestEvent expectedPayload = new();

            EventListener<TestEvent> listener = new(_ => { });

            listener.Trigger(expectedPayload);
            listener.Trigger(new AnotherTestEvent());

            Assert.AreSame(expectedPayload, listener.Payload);
        }

        [Test]
        public void Trigger_StoresPayloadData()
        {
            TestEvent payload = new(42, "Hello World");

            EventListener<TestEvent> listener = new(_ => { });

            listener.Trigger(payload);

            Assert.AreEqual(42, listener.Payload.Value);
            Assert.AreEqual("Hello World", listener.Payload.Message);
        }

        [Test]
        public void Trigger_CallbackReceivesPayloadData()
        {
            int receivedValue = 0;
            string receivedMessage = null;

            EventListener<TestEvent> listener = new(payload =>
            {
                receivedValue = payload.Value;
                receivedMessage = payload.Message;
            });

            listener.Trigger(new TestEvent(42, "Hello World"));

            Assert.AreEqual(42, receivedValue);
            Assert.AreEqual("Hello World", receivedMessage);
        }

        [Test]
        public async Task Register_SubscribesListenerToEventBusService()
        {
            EventBusService eventBus = new();

            await ServiceLocator.AddService(eventBus);

            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            listener.Register();

            eventBus.Trigger(new TestEvent());

            Assert.AreEqual(1, triggerCount);
        }

        [Test]
        public async Task Register_ReturnsSameListener()
        {
            await ServiceLocator.AddService(new EventBusService());

            EventListener<TestEvent> listener = new(_ => { });

            EventListener<TestEvent> result = listener.Register();

            Assert.AreSame(listener, result);
        }

        [Test]
        public async Task Register_WhenCalledMultipleTimes_DoesNotSubscribeMultipleTimes()
        {
            EventBusService eventBus = new();

            await ServiceLocator.AddService(eventBus);

            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            listener.Register();
            listener.Register();

            eventBus.Trigger(new TestEvent());

            Assert.AreEqual(1, triggerCount);
        }

        [Test]
        public async Task Unregister_UnsubscribesListenerFromEventBusService()
        {
            EventBusService eventBus = new();

            await ServiceLocator.AddService(eventBus);

            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            listener.Register();
            listener.Unregister();

            eventBus.Trigger(new TestEvent());

            Assert.AreEqual(0, triggerCount);
        }

        [Test]
        public async Task Unregister_ReturnsSameListener()
        {
            await ServiceLocator.AddService(new EventBusService());

            EventListener<TestEvent> listener = new(_ => { });

            EventListener<TestEvent> result = listener.Unregister();

            Assert.AreSame(listener, result);
        }

        [Test]
        public void Unregister_WhenEventBusServiceDoesNotExist_DoesNotThrow()
        {
            EventListener<TestEvent> listener = new(_ => { });

            Assert.DoesNotThrow(() => { listener.Unregister(); });
        }

        [Test]
        public async Task Unregister_WhenCalledMultipleTimes_DoesNotThrow()
        {
            await ServiceLocator.AddService(new EventBusService());

            EventListener<TestEvent> listener = new(_ => { });

            listener.Register();
            listener.Unregister();

            Assert.DoesNotThrow(() => { listener.Unregister(); });
        }

        [Test]
        public async Task Register_AfterUnregister_SubscribesAgain()
        {
            EventBusService eventBus = new();

            await ServiceLocator.AddService(eventBus);

            int triggerCount = 0;

            EventListener<TestEvent> listener = new(_ => { triggerCount++; });

            listener.Register();
            listener.Unregister();
            listener.Register();

            eventBus.Trigger(new TestEvent());

            Assert.AreEqual(1, triggerCount);
        }
    }
}