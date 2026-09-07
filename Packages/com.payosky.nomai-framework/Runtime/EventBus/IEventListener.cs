using System;

namespace NomaiFramework.EventBus
{
    /// <summary>
    /// Defines the contract for an event listener in the EventBus system.
    /// </summary>
    public interface IEventListener : IDisposable
    {
        event Action<IEvent> OnEventTriggered;
        Type PayloadType { get; }
        void Trigger(IEvent payload);
    }
}