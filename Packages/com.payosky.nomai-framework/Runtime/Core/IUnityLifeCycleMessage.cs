using System;

namespace NomaiFramework.Core.UnityLifeCycle
{
    public interface IUnityLifeCycleMessage { }

    public interface IUnityOnActivationMessage : IUnityLifeCycleMessage { }

    public interface IUnityOnDeactivationMessage : IUnityLifeCycleMessage { }

    public interface IUnityOnTickMessage : IUnityLifeCycleMessage { }

    [Serializable]
    public sealed class OnAwake : IUnityOnActivationMessage { }

    [Serializable]
    public sealed class OnEnable : IUnityOnActivationMessage { }

    [Serializable]
    public sealed class OnStart : IUnityOnActivationMessage { }

    [Serializable]
    public sealed class OnUpdate : IUnityOnTickMessage { }

    [Serializable]
    public sealed class OnLateUpdate : IUnityOnTickMessage { }

    [Serializable]
    public sealed class OnFixedUpdate : IUnityOnTickMessage { }

    [Serializable]
    public sealed class OnDisable : IUnityOnDeactivationMessage { }

    [Serializable]
    public sealed class OnDestroy : IUnityOnDeactivationMessage { }
}