using System;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Represents a state transition within a state machine.
    /// Defines the target state of the transition and the state machine it is associated with.
    /// Implementations may include logic to determine when the transition should occur.
    /// </summary>
    public interface IStateTransition : IDisposable
    {
        IStateMachine ManagedStateMachine { get; }
        IState TargetState { get; }
    }
}