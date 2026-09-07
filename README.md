<img width="1031" height="900" alt="image" src="https://github.com/user-attachments/assets/8a5b20cf-4b6a-4df6-b288-ba6944d50759" />

# Nomai Framework

**Nomai Framework** is a Unity framework designed around Clean Architecture principles, providing a lightweight foundation for bootstrapping projects quickly while preserving architectural integrity as they grow.

The framework provides reusable systems for common application-level concerns such as service management, event-driven communication, and state management, allowing projects to start with a consistent architectural foundation instead of rebuilding these systems from scratch.

## Requirements

* **Unity:** 6000.0 or newer
* **Package:** `com.payosky.nomai-framework`
* **Current Version:** `0.1.1`

## Installation

### Install from Git

Nomai Framework is distributed as a Unity Package Manager package located inside this repository.

In Unity:

1. Open **Window > Package Management > Package Manager**.
2. Click the **+** button.
3. Select **Install package from git URL...**
4. Enter:

```text
https://github.com/El-Calictivo/Nomai-Framework.git?path=/Packages/com.payosky.nomai-framework
```

Unity will install the package directly from the `Packages/com.payosky.nomai-framework` directory in this repository.

### Install through `manifest.json`

You can also add the package directly to your project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.payosky.nomai-framework": "https://github.com/El-Calictivo/Nomai-Framework.git?path=/Packages/com.payosky.nomai-framework"
  }
}
```

### Local Development

When working directly with a local copy of the repository, the package can be installed from disk:

1. Open **Window > Package Management > Package Manager**.
2. Click the **+** button.
3. Select **Install package from disk...**
4. Select:

```text
Packages/com.payosky.nomai-framework/package.json
```

This is useful when developing or testing changes to Nomai Framework locally.

## Dependencies

Nomai Framework currently depends on:

| Package                                       | Version  | Purpose                                      |
| --------------------------------------------- | -------- | -------------------------------------------- |
| [`com.cysharp.unitask`](https://github.com/cysharp/unitask)                         | `2.5.11` | Async/await integration optimized for Unity  | 
| [`com.mackysoft.serializereference-extensions`](https://github.com/mackysoft/Unity-SerializeReferenceExtensions) | `1.7.0`  | Improved `SerializeReference` editor support |
| [`com.unity.test-framework`](https://docs.unity3d.com/6000.6/Documentation/Manual/test-framework/test-framework-introduction.html)                    | `1.6.0`  | Unity Test Framework support                 |

These dependencies are declared by the package through its `package.json`.

Depending on how your Unity project resolves third-party packages, packages outside Unity's default registry may require their corresponding Git source or scoped registry to be configured.

## Features

Nomai Framework currently provides architectural building blocks including:

### Service Locator

Centralized registration and resolution of application services, allowing systems to depend on contracts rather than concrete implementations.

### Event Bus

Decoupled communication between systems through events and listeners without requiring direct references between unrelated features.

### State Management

Asynchronous state machines with explicit state lifecycle management:

```text
Enter → Tick → Exit
```

State changes can also be driven through reusable transitions such as:

* Event-driven transitions
* Unity Input System action transitions

## Repository Structure

```text
Nomai-Framework/
├── Packages/
│   └── com.payosky.nomai-framework/
│       ├── Runtime/
│       ├── Tests/
│       ├── package.json
│       ├── README.md
│       └── CHANGELOG.md
└── README.md
```

The framework itself lives under:

```text
Packages/com.payosky.nomai-framework
```

## Documentation

Detailed documentation and usage examples are available in the package [README](https://github.com/JuanDavidPF/Nomai-Framework/blob/main/Packages/com.payosky.nomai-framework/README.md
).


## Architecture Philosophy

Nomai Framework is built around a few core principles:

* Keep project bootstrap simple and explicit.
* Favor small systems with clear responsibilities.
* Depend on abstractions instead of concrete implementations.
* Reduce unnecessary coupling between application features.
* Provide reusable infrastructure without dictating gameplay architecture.
* Allow projects to scale without requiring architectural rewrites.

The framework aims to provide enough structure to keep a project maintainable while remaining flexible enough to adapt to different Unity applications and games.

## Author

Created by **Payosky**.

Website: http://www.payosky.dev
GitHub: https://github.com/JuanDavidPF
