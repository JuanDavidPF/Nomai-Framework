using Cysharp.Threading.Tasks;

namespace NomaiFramework.StateManagement
{
    /// <summary>
    /// Defines the interface for a state within a state machine.
    /// </summary>
    public interface IState
    {
        public IStateMachine ParentMachine { get; }
        public PlayerLoopTiming TickMode { get; }
        UniTask Enter(IStateMachine parentMachine);
        UniTask Exit();
        void Tick() { }
    }
}