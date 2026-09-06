using System;
using NomaiFramework.Services;
using UnityEngine;

namespace NomaiFramework.EventBus
{
    /// <summary>
    /// Represents an event listener that listens for events of a specific type
    /// and invokes a callback when the event is triggered.
    /// </summary>
    /// <typeparam name="TPayload">
    /// The type of event payload the listener handles. Must implement the <see cref="IEvent"/> interface.
    /// </typeparam>
    public sealed class EventListener<TPayload> : IEventListener, IDisposable where TPayload : IEvent
    {
        public Type PayloadType => typeof(TPayload);
        public TPayload Payload { get; private set; }

        public delegate void EventListenerCallback(TPayload payload);

        private readonly EventListenerCallback _callback;

        public EventListener(EventListenerCallback callback)
        {
            _callback = callback;
        }

        public void Trigger(IEvent payload)
        {
            if (payload is TPayload castedPayload) {
                Payload = castedPayload;
                _callback?.Invoke(castedPayload);
            }
#if UNITY_ENABLE_CHECKS
            else {
                Debug.LogWarning($"Event payload type mismatch! Expected {typeof(TPayload)}, got {payload.GetType()}.");
            }
#endif
        }

        public EventListener<TPayload> Register()
        {
            ServiceLocator.GetService<EventBusService>().Subscribe(this);
            return this;
        }

        public EventListener<TPayload> Unregister()
        {
            if (ServiceLocator.TryGetService(out EventBusService eventService)) {
                eventService.Unsubscribe(this);
            }

            return this;
        }

        public void Dispose()
        {
            Unregister();
        }
    }
}