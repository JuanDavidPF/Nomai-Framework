using System;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Represents a state machine transition triggered by an input action.
    /// </summary>
    public sealed class InputActionTransition : IStateTransition
    {
        public IStateMachine ManagedStateMachine { get; }
        public IState TargetState { get; }
        public event Action<IStateTransition> OnTransitionStarted;
        public event Action<IStateTransition> OnTransitionCompleted;

        private readonly InputAction _inputAction;

        public InputActionTransition(IStateMachine managedStateMachine, IState targetState, InputAction inputAction)
        {
            ManagedStateMachine = managedStateMachine;
            TargetState = targetState;
            _inputAction = inputAction;

            if (_inputAction == null) return;

            _inputAction.performed -= OnInputPerformed;
            _inputAction.performed += OnInputPerformed;
        }

        public InputActionTransition(IStateMachine managedStateMachine, IState targetState, InputActionReference inputAction)
            : this(managedStateMachine, targetState, inputAction?.action) { }

        private void OnInputPerformed(InputAction.CallbackContext context)
        {
            if (_inputAction != null) {
                _inputAction.performed -= OnInputPerformed;
            }

            OnTransitionStarted?.Invoke(this);
            ManagedStateMachine?.SetState(TargetState).Forget();
            OnTransitionCompleted?.Invoke(this);
        }

        public void Dispose()
        {
            if (_inputAction != null) {
                _inputAction.performed -= OnInputPerformed;
            }
        }
    }
}