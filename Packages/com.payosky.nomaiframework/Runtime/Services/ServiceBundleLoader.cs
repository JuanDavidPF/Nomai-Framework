using UnityEngine;

namespace NomaiFramework.Services
{
    /// <summary>
    /// The ServiceBundleLoader class is a sealed implementation of the BaseServiceLoader
    /// specifically designed to manage services bundled within a ServiceBundle object.
    /// It adds and removes services to the ServiceLocator during Unity lifecycle events
    /// based on its lifecycle hooks.
    /// </summary>
    public sealed class ServiceBundleLoader : BaseServiceLoader
    {
        [SerializeField] private ServiceBundle serviceBundle;

        public override void AddServices()
        {
            ServiceLocator.AddServices(serviceBundle?.services);
        }

        public override void RemoveServices()
        {
            ServiceLocator.RemoveServices(serviceBundle?.services);
        }
    }
}