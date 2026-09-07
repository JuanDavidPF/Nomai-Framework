using System;

namespace NomaiFramework.Core
{
    /// <summary>
    /// Represents an interface that provides a mechanism to expose the type signature of an implementing class.
    /// </summary>
    public interface ITypeSigned
    {
        Type TypeSignature { get; }
    }
}