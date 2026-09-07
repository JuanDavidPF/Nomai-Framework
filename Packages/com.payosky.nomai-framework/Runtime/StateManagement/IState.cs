using Cysharp.Threading.Tasks;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Defines the interface for a state within a state machine.
    /// </summary>
    public interface IState
    {
        public PlayerLoopTiming TickMode { get; }
        UniTask OnStateEnter(IStateMachine parentMachine);
        UniTask OnStateExit();
        void Tick() { }
    }
}