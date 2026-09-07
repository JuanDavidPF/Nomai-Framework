using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NomaiFramework.StateManagement;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace NomaiFramework.Tests.StateManagement
{
    public sealed class StateMachineTests
    {
        private StateMachine _stateMachine;

        [SetUp]
        public void Setup()
        {
            _stateMachine = new StateMachine();
        }

        [TearDown]
        public void TearDown()
        {
            _stateMachine?.Dispose();
        }

        [UnityTest]
        public IEnumerator SetState_EntersState()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestState state = new();

                await _stateMachine.SetState(state);

                Assert.That(_stateMachine.CurrentState, Is.SameAs(state));
                Assert.That(state.EnterCount, Is.EqualTo(1));
                Assert.That(state.ParentMachine, Is.SameAs(_stateMachine));
            });
        }

        [UnityTest]
        public IEnumerator SetState_ExitsPreviousState()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestState firstState = new();
                TestState secondState = new();

                await _stateMachine.SetState(firstState);
                await _stateMachine.SetState(secondState);

                Assert.That(firstState.ExitCount, Is.EqualTo(1));
                Assert.That(secondState.EnterCount, Is.EqualTo(1));
                Assert.That(_stateMachine.CurrentState, Is.SameAs(secondState));
            });
        }

        [UnityTest]
        public IEnumerator SetState_RaisesEventsInOrder()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestState firstState = new();
                TestState secondState = new();

                await _stateMachine.SetState(firstState);

                List<string> events = new();

                _stateMachine.OnStateRequested += (_, _) => events.Add("Requested");
                _stateMachine.OnStateExited += (_, _) => events.Add("Exited");
                _stateMachine.OnStateEntered += (_, _) => events.Add("Entered");

                await _stateMachine.SetState(secondState);

                Assert.That(events, Is.EqualTo(new[]
                {
                    "Requested",
                    "Exited",
                    "Entered"
                }));
            });
        }

        [UnityTest]
        public IEnumerator SetState_Null_ExitsCurrentState()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestState state = new();

                await _stateMachine.SetState(state);
                await _stateMachine.SetState(null);

                Assert.That(state.ExitCount, Is.EqualTo(1));
                Assert.That(_stateMachine.CurrentState, Is.Null);
            });
        }

        [UnityTest]
        public IEnumerator SetState_Null_DoesNotRaiseEntered()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestState state = new();

                await _stateMachine.SetState(state);

                int enteredCount = 0;

                _stateMachine.OnStateEntered += (_, _) => enteredCount++;

                await _stateMachine.SetState(null);

                Assert.That(enteredCount, Is.Zero);
            });
        }
    }
}