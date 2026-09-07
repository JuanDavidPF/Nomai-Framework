using System;
using Cysharp.Threading.Tasks;
using NomaiFramework.EventBus;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Represents a state machine transition triggered by events.
    /// </summary>
    public sealed class EventTransition : IStateTransition
    {
        public IStateMachine ManagedStateMachine { get; }
        public IState TargetState { get; }
        public event Action<IStateTransition> OnTransitionStarted;
        public event Action<IStateTransition> OnTransitionCompleted;

        private readonly IEventListener _eventListener;

        public EventTransition(IStateMachine managedStateMachine, IState targetState, IEventListener eventListener)
        {
            ManagedStateMachine = managedStateMachine;
            TargetState = targetState;
            _eventListener = eventListener;

            if (_eventListener == null) return;

            _eventListener.OnEventTriggered -= OnEventReceived;
            _eventListener.OnEventTriggered += OnEventReceived;
        }

        private void OnEventReceived(IEvent payload)
        {
            if (_eventListener != null) {
                _eventListener.OnEventTriggered -= OnEventReceived;
            }

            OnTransitionStarted?.Invoke(this);
            ManagedStateMachine?.SetState(TargetState).Forget();
            OnTransitionCompleted?.Invoke(this);
        }

        public void Dispose()
        {
            if (_eventListener != null) {
                _eventListener.OnEventTriggered -= OnEventReceived;
            }
        }
    }
}