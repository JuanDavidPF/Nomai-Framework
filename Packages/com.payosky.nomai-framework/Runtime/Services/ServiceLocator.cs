using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NomaiFramework.Services
{
    /// <summary>
    /// The ServiceLocator class provides a static implementation for the Service Locator pattern.
    /// It manages the registration, retrieval, and removal of service objects that implement the <see cref="IService"/> interface.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> Services = new();

        public static T GetService<T>() where T : IService
        {
            return (T)Services[typeof(T)];
        }

        public static async UniTask<T> AddService<T>(T service) where T : IService
        {
            if (service == null) {
#if UNITY_ENABLE_CHECKS
                Debug.LogWarning("Service to add is null!");
#endif
                return default;
            }

            if (Services.TryAdd(service.TypeSignature, service)) {
                await service.OnServiceLoad();
#if DEBUG
                Debug.Log($"Service {service.TypeSignature} registered.");
#endif
                return service;
            }

#if UNITY_ENABLE_CHECKS
            Debug.LogWarning($"IService {service.TypeSignature} already registered!");
#endif

            return (T)Services[service.TypeSignature];
        }

        public static UniTask AddServices(params IService[] services)
        {
            if (services == null) return UniTask.CompletedTask;

            List<UniTask> loadServiceOps = new();

            foreach (IService service in services) {
                loadServiceOps.Add(AddService(service));
            }

            return UniTask.WhenAll(loadServiceOps);
        }

        public static bool TryGetService<T>(out T service) where T : IService
        {
            if (Services.TryGetValue(typeof(T), out IService registeredService) && registeredService is T typedService) {
                service = typedService;
                return true;
            }

            service = default;
            return false;
        }

        public static async UniTask<T> RemoveService<T>() where T : IService
        {
            if (!Services.TryGetValue(typeof(T), out IService service)) return default;
            await RemoveService(service);
            return (T)service;
        }

        public static async UniTask<IService> RemoveService(IService service)
        {
            if (service == null) {
#if UNITY_ENABLE_CHECKS
                Debug.LogWarning("Service to remove is null!");
#endif
                return null;
            }

            if (!Services.TryGetValue(service.TypeSignature, out IService registeredService)) return null;
            if (!ReferenceEquals(service, registeredService)) return null;

            Services.Remove(service.TypeSignature);
#if DEBUG
            Debug.Log($"Service {service.TypeSignature} removed.");
#endif
            await registeredService.OnServiceDispose();
            return registeredService;
        }

        public static async UniTask RemoveServices(params IService[] services)
        {
            if (services == null) return;

            List<UniTask> disposeServiceOps = new();

            foreach (IService service in services) {
                disposeServiceOps.Add(RemoveService(service));
            }

            await UniTask.WhenAll(disposeServiceOps);
        }

        public static bool HasService<T>() where T : IService
        {
            return Services.ContainsKey(typeof(T));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Dispose()
        {
            foreach (IService service in Services.Values) {
                service.OnServiceDispose();
            }

            Services.Clear();
        }
    }
}