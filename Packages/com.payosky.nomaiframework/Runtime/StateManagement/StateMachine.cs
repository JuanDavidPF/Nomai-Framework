using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Represents a state machine that manages transitions between states implementing the <see cref="IState"/> interface.
    /// </summary>
    public sealed class StateMachine : IStateMachine, IDisposable
    {
        public IState CurrentState { get; private set; }
        public event Action<IStateMachine, IState> OnStateRequested;
        public event Action<IStateMachine, IState> OnStateExited;
        public event Action<IStateMachine, IState> OnStateEntered;

        public async UniTask SetState(IState state)
        {
            OnStateRequested?.Invoke(this, state);
            await ExitCurrentState();
            CurrentState = state;
            await EnterCurrentState();
            StartStateTick().Forget();
        }

        private async UniTask ExitCurrentState()
        {
            if (CurrentState == null) return;
# if DEBUG
            Debug.Log($"Exiting state: {CurrentState.GetType().Name}");
#endif
            await CurrentState.Exit();
            OnStateExited?.Invoke(this, CurrentState);
        }

        private async UniTask EnterCurrentState()
        {
            if (CurrentState == null) return;
# if DEBUG
            Debug.Log($"Entering state: {CurrentState.GetType().Name}");
#endif
            await CurrentState.Enter(this);
            OnStateEntered?.Invoke(this, CurrentState);
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