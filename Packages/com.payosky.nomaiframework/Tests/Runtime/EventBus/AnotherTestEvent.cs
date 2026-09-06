using NomaiFramework.EventBus;

namespace NomaiFramework.Tests.EventBus
{
    /// <summary>
    /// Represents an event used for testing purposes within the event bus system.
    /// </summary>
    internal sealed class AnotherTestEvent : IEvent
    {
        public int Value { get; }
        public string Message { get; }

        public AnotherTestEvent() { }

        public AnotherTestEvent(int value, string message)
        {
            Value = value;
            Message = message;
        }
    }
}