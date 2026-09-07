# Nomai Framework

**Nomai Framework** is a lightweight Unity framework for bootstrapping projects with a clean and scalable architecture.

The goal is to provide a small set of reusable architectural systems that make it faster to start and grow Unity projects without compromising code quality, separation of concerns, or maintainability.

## Core Features

### Service Locator

The Service Locator provides a centralized way to register and access shared services across the application.

Use it during your project bootstrap to register dependencies such as managers, repositories, configuration providers, or other application-level services.

```csharp
// Register application services during bootstrap.
ServiceLocator.Register<IMyService>(new MyService());

// Resolve them where required.
IMyService service = ServiceLocator.Get<IMyService>();
```

The intention is to keep service creation centralized while allowing systems to depend on abstractions rather than concrete implementations.

---

### Event Bus

The Event Bus provides decoupled communication between systems through events.

Instead of creating direct dependencies between unrelated features, systems can publish events while listeners react independently.

```csharp
public sealed class PlayerCompletedEvent : IEvent
{
}
```

A listener can subscribe to an event and react when it is triggered.

This is especially useful for communication between gameplay systems, UI, state management, analytics, and other features that should remain independent from each other.

Events can also be connected directly to state transitions through `EventTransition`.

```csharp
new EventTransition(
    stateMachine,
    nextState,
    eventListener
);
```

When the event is received, the transition automatically requests the target state and unsubscribes itself.

---

### State Management

`StateMachine` provides an asynchronous state lifecycle built around `IState`.

Each state can define its own:

```csharp
Enter()
Exit()
Tick()
```

Changing states is asynchronous:

```csharp
await stateMachine.SetState(nextState);
```

The state machine handles the complete lifecycle:

```text
State Requested
      ↓
Current State Exit
      ↓
Target State Assigned
      ↓
Target State Enter
      ↓
Target State Tick
```

It also exposes lifecycle events:

```csharp
stateMachine.OnStateRequested += OnStateRequested;
stateMachine.OnStateExited += OnStateExited;
stateMachine.OnStateEntered += OnStateEntered;
```

### State Transitions

Transitions allow states to change in response to external signals without putting that logic directly inside the state.

#### Event Transition

```csharp
new EventTransition(
    stateMachine,
    nextState,
    eventListener
);
```

#### Input Action Transition

Unity's New Input System can also trigger transitions:

```csharp
new InputActionTransition(
    stateMachine,
    nextState,
    inputAction
);
```

When the configured `InputAction` is performed, the state machine transitions to the target state.

This makes transitions reusable and keeps states focused on their own behavior rather than on the source of the transition.

---

## Architecture Philosophy

Nomai Framework favors a few simple principles:

* **Bootstrap explicitly.** Application dependencies should have a clear composition point.
* **Depend on abstractions.** Systems should communicate through interfaces and contracts whenever possible.
* **Keep systems decoupled.** Events and transitions prevent unnecessary direct dependencies.
* **Keep responsibilities small.** States, services, events, and transitions each solve one specific problem.
* **Scale without rewriting the foundation.** Projects should be able to grow in complexity without requiring architectural shortcuts.

Nomai Framework is intentionally small. It provides the architectural foundation while leaving project-specific gameplay and application logic to the project using it.
