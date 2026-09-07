using NomaiFramework.EventBus;

namespace NomaiFramework.Tests.EventBus
{
    /// <summary>
    /// Represents an event used for testing purposes within the event bus system.
    /// </summary>
    internal sealed class TestEvent : IEvent
    {
        public int Value { get; }
        public string Message { get; }

        public TestEvent() { }

        public TestEvent(int value, string message)
        {
            Value = value;
            Message = message;
        }
    }
}