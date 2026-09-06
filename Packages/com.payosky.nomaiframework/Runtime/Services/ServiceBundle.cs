using UnityEngine;

namespace NomaiFramework.Services
{
    /// <summary>
    /// The ServiceBundle class acts as a container for a collection of services
    /// that implement the <see cref="IService"/> interface. It is designed to
    /// facilitate managing multiple services as a single unit, often in
    /// conjunction with a service loader such as <see cref="ServiceBundleLoader"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "ServiceBundle", menuName = "Nomai/Service/ServiceBundle")]
    public class ServiceBundle : ScriptableObject
    {
        [SerializeReference] [SubclassSelector]
        public IService[] services;
    }
}