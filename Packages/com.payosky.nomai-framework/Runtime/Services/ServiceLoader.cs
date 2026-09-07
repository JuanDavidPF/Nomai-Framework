using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NomaiFramework.Services
{
    /// <summary>
    /// The ServiceLoader class is responsible for managing the lifecycle of services
    /// in the application by integrating with the ServiceLocator. It extends
    /// BaseServiceLoader and provides concrete implementations for initializing
    /// and cleaning up services during specific Unity lifecycle events.
    /// </summary>
    public sealed class ServiceLoader : BaseServiceLoader
    {
        [SerializeReference] [SubclassSelector]
        private IService[] services;

        public override async UniTask AddServices()
        {
            await ServiceLocator.AddServices(services);
        }

        public override async UniTask RemoveServices()
        {
            await ServiceLocator.RemoveServices(services);
        }
    }
}