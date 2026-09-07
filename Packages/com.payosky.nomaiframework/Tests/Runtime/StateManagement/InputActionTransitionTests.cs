using NomaiFramework.StateManagement;
using NUnit.Framework;
using UnityEngine.InputSystem;

namespace NomaiFramework.Tests.StateManagement
{
    public sealed class InputActionTransitionTests : InputTestFixture
    {
        private StateMachine _stateMachine;
        private Keyboard _keyboard;
        private InputAction _inputAction;
        private InputActionTransition _transition;

        public override void Setup()
        {
            base.Setup();

            _stateMachine = new StateMachine();
            _keyboard = InputSystem.AddDevice<Keyboard>();

            _inputAction = new InputAction(
                "Submit",
                InputActionType.Button,
                "<Keyboard>/space"
            );

            _inputAction.Enable();
        }

        public override void TearDown()
        {
            _transition?.Dispose();
            _inputAction?.Dispose();
            _stateMachine?.Dispose();

            base.TearDown();
        }

        [Test]
        public void InputAction_Performed_TransitionsToTargetState()
        {
            TestState targetState = new();

            _transition = new InputActionTransition(
                _stateMachine,
                targetState,
                _inputAction
            );

            Press(_keyboard.spaceKey);

            Assert.That(_stateMachine.CurrentState, Is.SameAs(targetState));
            Assert.That(targetState.EnterCount, Is.EqualTo(1));
        }

        [Test]
        public void InputAction_Performed_TransitionsOnlyOnce()
        {
            TestState targetState = new();

            _transition = new InputActionTransition(
                _stateMachine,
                targetState,
                _inputAction
            );

            Press(_keyboard.spaceKey);
            Release(_keyboard.spaceKey);

            Press(_keyboard.spaceKey);

            Assert.That(_stateMachine.CurrentState, Is.SameAs(targetState));
            Assert.That(targetState.EnterCount, Is.EqualTo(1));
        }

        [Test]
        public void Dispose_PreventsTransition()
        {
            TestState targetState = new();

            _transition = new InputActionTransition(
                _stateMachine,
                targetState,
                _inputAction
            );

            _transition.Dispose();

            Press(_keyboard.spaceKey);

            Assert.That(_stateMachine.CurrentState, Is.Null);
            Assert.That(targetState.EnterCount, Is.Zero);
        }

        [Test]
        public void Constructor_WithNullInputAction_DoesNotThrow()
        {
            TestState targetState = new();

            Assert.DoesNotThrow(() =>
            {
                _transition = new InputActionTransition(
                    _stateMachine,
                    targetState,
                    (InputAction)null
                );
            });
        }

        [Test]
        public void InputAction_Performed_RaisesStateMachineEvents()
        {
            TestState targetState = new();

            IState requestedState = null;
            IState enteredState = null;

            _stateMachine.OnStateRequested += (_, state) => requestedState = state;
            _stateMachine.OnStateEntered += (_, state) => enteredState = state;

            _transition = new InputActionTransition(
                _stateMachine,
                targetState,
                _inputAction
            );

            Press(_keyboard.spaceKey);

            Assert.That(requestedState, Is.SameAs(targetState));
            Assert.That(enteredState, Is.SameAs(targetState));
        }
    }
}