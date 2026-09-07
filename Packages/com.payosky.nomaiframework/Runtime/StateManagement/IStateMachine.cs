using System;
using Cysharp.Threading.Tasks;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Represents a state machine that manages transitions between states implementing the <see cref="IState"/> interface.
    /// </summary>
    public interface IStateMachine
    {
        IState CurrentState { get; }
        event Action<IStateMachine, IState> OnStateRequested;
        event Action<IStateMachine, IState> OnStateExited;
        event Action<IStateMachine, IState> OnStateEntered;
        UniTask SetState(IState state);
    }
}