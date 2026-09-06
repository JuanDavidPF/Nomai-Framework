using System;
using System.Collections.Generic;
using NUnit.Framework;
using NomaiFramework.Services;

namespace NomaiFramework.Tests.Services
{
    public class ServiceLocatorTests
    {
        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Dispose();
        }

        [Test]
        public void AddService_RegistersService()
        {
            TestService service = new();

            ServiceLocator.AddService(service);

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public void AddService_LoadsService()
        {
            TestService service = new();

            ServiceLocator.AddService(service);

            Assert.AreEqual(1, service.LoadCount);
        }

        [Test]
        public void AddService_ReturnsRegisteredService()
        {
            TestService service = new();

            TestService result = ServiceLocator.AddService(service);

            Assert.AreSame(service, result);
        }

        [Test]
        public void AddService_WhenServiceAlreadyExists_DoesNotReplaceService()
        {
            TestService firstService = new();
            TestService secondService = new();

            ServiceLocator.AddService(firstService);
            TestService result = ServiceLocator.AddService(secondService);

            Assert.AreSame(firstService, result);
            Assert.AreSame(firstService, ServiceLocator.GetService<TestService>());
        }

        [Test]
        public void AddService_WhenServiceAlreadyExists_DoesNotLoadNewService()
        {
            TestService firstService = new();
            TestService secondService = new();

            ServiceLocator.AddService(firstService);
            ServiceLocator.AddService(secondService);

            Assert.AreEqual(1, firstService.LoadCount);
            Assert.AreEqual(0, secondService.LoadCount);
        }

        [Test]
        public void AddServices_RegistersAllServices()
        {
            TestService firstService = new();
            AnotherTestService secondService = new();

            ServiceLocator.AddServices(firstService, secondService);

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
            Assert.IsTrue(ServiceLocator.HasService<AnotherTestService>());
        }

        [Test]
        public void AddServices_LoadsAllServices()
        {
            TestService firstService = new();
            AnotherTestService secondService = new();

            ServiceLocator.AddServices(firstService, secondService);

            Assert.AreEqual(1, firstService.LoadCount);
            Assert.AreEqual(1, secondService.LoadCount);
        }

        [Test]
        public void GetService_ReturnsRegisteredService()
        {
            TestService service = new();
            ServiceLocator.AddService(service);

            TestService result = ServiceLocator.GetService<TestService>();

            Assert.AreSame(service, result);
        }

        [Test]
        public void GetService_WhenServiceDoesNotExist_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => { ServiceLocator.GetService<TestService>(); });
        }

        [Test]
        public void TryGetService_WhenServiceExists_ReturnsTrue()
        {
            TestService service = new();
            ServiceLocator.AddService(service);

            bool result = ServiceLocator.TryGetService(out TestService registeredService);

            Assert.IsTrue(result);
            Assert.AreSame(service, registeredService);
        }

        [Test]
        public void TryGetService_WhenServiceDoesNotExist_ReturnsFalse()
        {
            bool result = ServiceLocator.TryGetService(out TestService service);

            Assert.IsFalse(result);
            Assert.IsNull(service);
        }

        [Test]
        public void HasService_WhenServiceExists_ReturnsTrue()
        {
            ServiceLocator.AddService(new TestService());

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public void HasService_WhenServiceDoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public void RemoveService_RemovesService()
        {
            TestService service = new();
            ServiceLocator.AddService(service);

            ServiceLocator.RemoveService<TestService>();

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public void RemoveService_DisposesService()
        {
            TestService service = new();
            ServiceLocator.AddService(service);

            ServiceLocator.RemoveService<TestService>();

            Assert.AreEqual(1, service.DisposeCount);
        }

        [Test]
        public void RemoveService_WhenServiceDoesNotExist_DoesNothing()
        {
            Assert.DoesNotThrow(() => { ServiceLocator.RemoveService<TestService>(); });
        }

        [Test]
        public void RemoveService_ByInstance_RemovesService()
        {
            TestService service = new();
            ServiceLocator.AddService(service);

            ServiceLocator.RemoveService(service);

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public void RemoveService_ByInstance_DisposesService()
        {
            TestService service = new();
            ServiceLocator.AddService(service);

            ServiceLocator.RemoveService(service);

            Assert.AreEqual(1, service.DisposeCount);
        }

        [Test]
        public void RemoveServices_RemovesAllSpecifiedServices()
        {
            TestService firstService = new();
            AnotherTestService secondService = new();

            ServiceLocator.AddServices(firstService, secondService);

            ServiceLocator.RemoveServices(firstService, secondService);

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
            Assert.IsFalse(ServiceLocator.HasService<AnotherTestService>());
        }

        [Test]
        public void RemoveService_WhenInstanceIsNotRegistered_DoesNotRemoveRegisteredService()
        {
            TestService registeredService = new();
            TestService differentInstance = new();

            ServiceLocator.AddService(registeredService);

            ServiceLocator.RemoveService(differentInstance);

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
            Assert.AreSame(registeredService, ServiceLocator.GetService<TestService>());
            Assert.AreEqual(0, registeredService.DisposeCount);
            Assert.AreEqual(0, differentInstance.DisposeCount);
        }

        [Test]
        public void RemoveServices_DisposesAllSpecifiedServices()
        {
            TestService firstService = new();
            AnotherTestService secondService = new();

            ServiceLocator.AddServices(firstService, secondService);

            ServiceLocator.RemoveServices(firstService, secondService);

            Assert.AreEqual(1, firstService.DisposeCount);
            Assert.AreEqual(1, secondService.DisposeCount);
        }

        [Test]
        public void Dispose_DisposesAllServices()
        {
            TestService firstService = new();
            AnotherTestService secondService = new();

            ServiceLocator.AddServices(firstService, secondService);

            ServiceLocator.Dispose();

            Assert.AreEqual(1, firstService.DisposeCount);
            Assert.AreEqual(1, secondService.DisposeCount);
        }

        [Test]
        public void Dispose_RemovesAllServices()
        {
            ServiceLocator.AddServices(
                new TestService(),
                new AnotherTestService()
            );

            ServiceLocator.Dispose();

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
            Assert.IsFalse(ServiceLocator.HasService<AnotherTestService>());
        }

        [Test]
        public void Dispose_WhenCalledMultipleTimes_DoesNotDisposeServiceMultipleTimes()
        {
            TestService service = new();
            ServiceLocator.AddService(service);

            ServiceLocator.Dispose();
            ServiceLocator.Dispose();

            Assert.AreEqual(1, service.DisposeCount);
        }

        private sealed class TestService : IService
        {
            public Type TypeSignature => typeof(TestService);

            public int LoadCount { get; private set; }
            public int DisposeCount { get; private set; }

            public void Load()
            {
                LoadCount++;
            }

            public void Dispose()
            {
                DisposeCount++;
            }
        }

        private sealed class AnotherTestService : IService
        {
            public Type TypeSignature => typeof(AnotherTestService);

            public int LoadCount { get; private set; }
            public int DisposeCount { get; private set; }

            public void Load()
            {
                LoadCount++;
            }

            public void Dispose()
            {
                DisposeCount++;
            }
        }
    }
}