using System;
using Cysharp.Threading.Tasks;
using NomaiFramework.Services;
using UnityEngine;

namespace NomaiFramework.Tests.Services
{
    /// <summary>
    /// Implements the <see cref="IService"/> interface and provides a concrete service
    /// within the framework. This service is responsible for managing resource loading
    /// and disposal operations while tracking its load and dispose counts for diagnostic purposes.
    /// </summary>
    [Serializable]
    internal sealed class AnotherTestService : IService
    {
        public Type TypeSignature => typeof(AnotherTestService);

        public string loadSource = "{Load Source}";
        public int LoadCount { get; private set; }
        public int DisposeCount { get; private set; }

        public UniTask OnServiceLoad()
        {
            LoadCount++;
#if DEBUG
            Debug.Log($"{nameof(TypeSignature)} loaded! From: {loadSource}");
#endif
            return UniTask.CompletedTask;
        }

        public UniTask OnServiceDispose()
        {
            DisposeCount++;

#if DEBUG
            Debug.Log($"{nameof(TypeSignature)} disposed!");
#endif
            return UniTask.CompletedTask;
        }
    }
}