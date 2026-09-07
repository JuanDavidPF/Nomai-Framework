using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        public async Task AddService_RegistersService()
        {
            TestService service = new();

            await ServiceLocator.AddService(service);

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public async Task AddService_LoadsService()
        {
            TestService service = new();

            await ServiceLocator.AddService(service);

            Assert.AreEqual(1, service.LoadCount);
        }

        [Test]
        public async Task AddService_ReturnsRegisteredService()
        {
            TestService service = new();

            TestService result = await ServiceLocator.AddService(service);

            Assert.AreSame(service, result);
        }

        [Test]
        public async Task AddService_WhenServiceAlreadyExists_DoesNotReplaceService()
        {
            TestService firstService = new();
            TestService secondService = new();

            await ServiceLocator.AddService(firstService);
            TestService result = await ServiceLocator.AddService(secondService);

            Assert.AreSame(firstService, result);
            Assert.AreSame(firstService, ServiceLocator.GetService<TestService>());
        }

        [Test]
        public async Task AddService_WhenServiceAlreadyExists_DoesNotLoadNewService()
        {
            TestService firstService = new();
            TestService secondService = new();

            await ServiceLocator.AddService(firstService);
            await ServiceLocator.AddService(secondService);

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
        public async Task GetService_ReturnsRegisteredService()
        {
            TestService service = new();
            await ServiceLocator.AddService(service);

            TestService result = ServiceLocator.GetService<TestService>();

            Assert.AreSame(service, result);
        }

        [Test]
        public void GetService_WhenServiceDoesNotExist_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => { ServiceLocator.GetService<TestService>(); });
        }

        [Test]
        public async Task TryGetService_WhenServiceExists_ReturnsTrue()
        {
            TestService service = new();
            await ServiceLocator.AddService(service);

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
        public async Task HasService_WhenServiceExists_ReturnsTrue()
        {
            await ServiceLocator.AddService(new TestService());

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public void HasService_WhenServiceDoesNotExist_ReturnsFalse()
        {
            Assert.IsFalse(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public async Task RemoveService_RemovesService()
        {
            TestService service = new();
            await ServiceLocator.AddService(service);

            await ServiceLocator.RemoveService<TestService>();

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public async Task RemoveService_DisposesService()
        {
            TestService service = new();
            await ServiceLocator.AddService(service);

            await ServiceLocator.RemoveService<TestService>();

            Assert.AreEqual(1, service.DisposeCount);
        }

        [Test]
        public void RemoveService_WhenServiceDoesNotExist_DoesNothing()
        {
            Assert.DoesNotThrow(() => { ServiceLocator.RemoveService<TestService>().Forget(); });
        }

        [Test]
        public async Task RemoveService_ByInstance_RemovesService()
        {
            TestService service = new();
            await ServiceLocator.AddService(service);

            await ServiceLocator.RemoveService(service);

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
        }

        [Test]
        public async Task RemoveService_ByInstance_DisposesService()
        {
            TestService service = new();
            await ServiceLocator.AddService(service);

            await ServiceLocator.RemoveService(service);

            Assert.AreEqual(1, service.DisposeCount);
        }

        [Test]
        public async Task RemoveServices_RemovesAllSpecifiedServices()
        {
            TestService firstService = new();
            AnotherTestService secondService = new();

            await ServiceLocator.AddServices(firstService, secondService);

            await ServiceLocator.RemoveServices(firstService, secondService);

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
            Assert.IsFalse(ServiceLocator.HasService<AnotherTestService>());
        }

        [Test]
        public async Task RemoveService_WhenInstanceIsNotRegistered_DoesNotRemoveRegisteredService()
        {
            TestService registeredService = new();
            TestService differentInstance = new();

            await ServiceLocator.AddService(registeredService);

            await ServiceLocator.RemoveService(differentInstance);

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
            Assert.AreSame(registeredService, ServiceLocator.GetService<TestService>());
            Assert.AreEqual(0, registeredService.DisposeCount);
            Assert.AreEqual(0, differentInstance.DisposeCount);
        }

        [Test]
        public async Task RemoveServices_DisposesAllSpecifiedServices()
        {
            TestService firstService = new();
            AnotherTestService secondService = new();

            await ServiceLocator.AddServices(firstService, secondService);

            await ServiceLocator.RemoveServices(firstService, secondService);

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
        public async Task Dispose_WhenCalledMultipleTimes_DoesNotDisposeServiceMultipleTimes()
        {
            TestService service = new();
            await ServiceLocator.AddService(service);

            ServiceLocator.Dispose();
            ServiceLocator.Dispose();

            Assert.AreEqual(1, service.DisposeCount);
        }
    }
}