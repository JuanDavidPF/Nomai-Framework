using Cysharp.Threading.Tasks;
using NomaiFramework.Core;

namespace NomaiFramework.Services
{
    /// <summary>
    /// Represents a service interface that defines the contract for services within the framework.
    /// </summary>
    public interface IService : ITypeSigned
    {
        UniTask OnServiceLoad()
        {
            return default;
        }

        UniTask OnServiceDispose()
        {
            return default;
        }
    }
}