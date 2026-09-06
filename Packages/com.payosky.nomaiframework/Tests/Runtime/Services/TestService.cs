using System;
using NomaiFramework.Services;
using UnityEngine;

namespace NomaiFramework.Tests.Services
{
    /// <summary>
    /// Represents a specific implementation of the IService interface within the framework.
    /// This service provides functionality related to loading and disposing resources
    /// and exposes its type signature for identification purposes.
    /// </summary>
    [Serializable]
    public sealed class TestService : IService
    {
        public Type TypeSignature => typeof(TestService);

        public string loadSource = "{Load Source}";
        public int LoadCount { get; private set; }
        public int DisposeCount { get; private set; }

        public void Load()
        {
            LoadCount++;
#if DEBUG
            Debug.Log($"{nameof(TypeSignature)} loaded! From: {loadSource}");
#endif
        }

        public void Dispose()
        {
            DisposeCount++;

#if DEBUG
            Debug.Log($"{nameof(TypeSignature)} disposed!");
#endif
        }
    }
}