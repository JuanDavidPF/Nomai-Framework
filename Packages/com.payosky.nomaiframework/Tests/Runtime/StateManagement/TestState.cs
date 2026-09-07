using Cysharp.Threading.Tasks;
using NomaiFramework.StateManagement;

namespace NomaiFramework.Tests.StateManagement
{
    /// <summary>
    /// Represents a state within a state machine.
    /// </summary>
    internal sealed class TestState : IState
    {
        public IStateMachine ParentMachine { get; private set; }
        public PlayerLoopTiming TickMode => PlayerLoopTiming.Update;

        public int EnterCount { get; private set; }
        public int ExitCount { get; private set; }
        public int TickCount { get; private set; }

        public UniTask Enter(IStateMachine stateMachine)
        {
            ParentMachine = stateMachine;
            EnterCount++;

            return UniTask.CompletedTask;
        }

        public UniTask Exit()
        {
            ExitCount++;

            return UniTask.CompletedTask;
        }

        public void Tick()
        {
            TickCount++;
        }
    }
}