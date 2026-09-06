using NomaiFramework.Core.UnityLifeCycle;
using UnityEngine;

namespace NomaiFramework.Services
{
    /// <summary>
    /// Represents an abstract base class responsible for managing the lifecycle of services
    /// in a Unity application. Provides methods for adding and removing services based on
    /// specified Unity lifecycle events.
    /// </summary>
    public abstract class BaseServiceLoader : MonoBehaviour
    {
        [SerializeReference] [SubclassSelector]
        private IUnityOnActivationMessage loadServicesHook;

        [SerializeReference] [SubclassSelector]
        private IUnityOnDeactivationMessage disposeServicesHook;

        public abstract void AddServices();

        public abstract void RemoveServices();

        public void Awake()
        {
            if (loadServicesHook is OnAwake) {
                AddServices();
            }
        }

        public void OnEnable()
        {
            if (loadServicesHook is OnEnable) {
                AddServices();
            }
        }

        public void Start()
        {
            if (loadServicesHook is OnStart) {
                AddServices();
            }
        }

        public void OnDisable()
        {
            if (disposeServicesHook is OnDisable) {
                RemoveServices();
            }
        }

        public void OnDestroy()
        {
            if (disposeServicesHook is OnDestroy) {
                RemoveServices();
            }
        }
    }
}