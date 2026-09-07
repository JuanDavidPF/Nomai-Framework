using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Represents a state machine that manages transitions between states implementing the <see cref="IState"/> interface.
    /// </summary>
    public sealed class StateMachine : IStateMachine
    {
        public event Action<IStateMachine, IState, IState> OnStateChangeRequested;
        public event Action<IStateMachine, IState, IState> OnStateChanged;
        public event Action<IStateMachine, IState> OnStateUnloadStarted;
        public event Action<IStateMachine, IState> OnStateUnloadFinished;
        public event Action<IStateMachine, IState> OnStateLoadStarted;
        public event Action<IStateMachine, IState> OnStateLoadFinished;

        public IState PreviousState { get; private set; }
        public IState CurrentState { get; private set; }

        public async UniTask SetState(IState newState)
        {
            OnStateChangeRequested?.Invoke(this, CurrentState, newState);
            await ExitCurrentState();
            PreviousState = CurrentState;
            CurrentState = newState;
            await EnterCurrentState();
            StartStateTick().Forget();
            OnStateChanged?.Invoke(this, PreviousState, CurrentState);
        }

        private async UniTask ExitCurrentState()
        {
            if (CurrentState == null) return;
# if DEBUG
            Debug.Log($"Exiting state: {CurrentState.GetType().Name}");
#endif
            OnStateUnloadStarted?.Invoke(this, CurrentState);
            await CurrentState.OnStateExit();
            OnStateUnloadFinished?.Invoke(this, CurrentState);
        }

        private async UniTask EnterCurrentState()
        {
            if (CurrentState == null) return;
# if DEBUG
            Debug.Log($"Entering state: {CurrentState.GetType().Name}");
#endif
            OnStateLoadStarted?.Invoke(this, CurrentState);
            await CurrentState.OnStateEnter(this);
            OnStateLoadFinished?.Invoke(this, CurrentState);
        }

        private async UniTaskVoid StartStateTick()
        {
            if (CurrentState == null) return;

            IState state = CurrentState;

            while (CurrentState == state) {
                state.Tick();
                await UniTask.Yield(state.TickMode);
            }
        }

        public void Dispose()
        {
            SetState(null).Forget();
        }
    }
}