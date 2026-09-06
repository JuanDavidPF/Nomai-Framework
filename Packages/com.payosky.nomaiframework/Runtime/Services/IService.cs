using System;
using NomaiFramework.Core;

namespace NomaiFramework.Services
{
    /// <summary>
    /// Represents a service interface that defines the contract for services within the framework.
    /// </summary>
    public interface IService : ITypeSigned, IDisposable
    {
        void Load();
    }
}