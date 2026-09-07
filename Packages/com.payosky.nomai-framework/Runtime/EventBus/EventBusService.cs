using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NomaiFramework.Services;
using UnityEngine;

namespace NomaiFramework.EventBus
{
    /// <summary>
    /// Provides an implementation of the <see cref="IService"/> interface that manages an event bus system,
    /// enabling decoupled communication between different parts of an application via custom event payloads.
    /// </summary>
    [Serializable]
    public sealed class EventBusService : IService
    {
        public Type TypeSignature => typeof(EventBusService);
        private readonly HashSet<IEventListener> _eventListeners = new();
        private readonly Dictionary<Type, Action<IEvent>> _eventSubscriptionsActions = new();
        public event Action<IEvent> OnEventDispatched;

        public void Subscribe(IEventListener listener)
        {
            if (listener == null) return;
            if (!_eventListeners.Add(listener)) return;

            Type payloadType = listener.PayloadType;
            if (_eventSubscriptionsActions.TryGetValue(payloadType, out Action<IEvent> subscriptions)) {
                _eventSubscriptionsActions[payloadType] = subscriptions + listener.Trigger;
            }
            else {
                _eventSubscriptionsActions.Add(payloadType, listener.Trigger);
            }
        }

        public void Unsubscribe(IEventListener listener)
        {
            if (listener == null) return;
            if (!_eventListeners.Remove(listener)) return;

            Type payloadType = listener.PayloadType;
            if (_eventSubscriptionsActions.TryGetValue(payloadType, out Action<IEvent> subscriptions)) {
                _eventSubscriptionsActions[payloadType] = subscriptions - listener.Trigger;
            }
        }

        public void Trigger<T>(T eventPayload) where T : IEvent
        {
            if (eventPayload == null) return;
# if DEBUG
            Debug.Log($"Event Triggered: {eventPayload.GetType().Name}");
#endif
            OnEventDispatched?.Invoke(eventPayload);
            if (_eventSubscriptionsActions.TryGetValue(eventPayload.GetType(), out Action<IEvent> subscriptions)) {
                subscriptions?.Invoke(eventPayload);
            }
        }

        public UniTask OnServiceDispose()
        {
            _eventSubscriptionsActions.Clear();
            _eventListeners.Clear();
            OnEventDispatched = null;
            return UniTask.CompletedTask;
        }
    }
}