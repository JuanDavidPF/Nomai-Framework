using System;
using Cysharp.Threading.Tasks;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Represents a state machine that manages transitions between states implementing the <see cref="IState"/> interface.
    /// </summary>
    public interface IStateMachine : IDisposable
    {
        public event Action<IStateMachine, IState, IState> OnStateChangeRequested;
        public event Action<IStateMachine, IState, IState> OnStateChanged;
        public event Action<IStateMachine, IState> OnStateUnloadStarted;
        public event Action<IStateMachine, IState> OnStateUnloadFinished;
        public event Action<IStateMachine, IState> OnStateLoadStarted;
        public event Action<IStateMachine, IState> OnStateLoadFinished;

        IState PreviousState { get; }
        IState CurrentState { get; }

        UniTask SetState(IState newState);
    }
}