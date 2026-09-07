using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NomaiFramework.Core.UnityLifeCycle;
using NomaiFramework.Services;
using NUnit.Framework;
using UnityEngine;

namespace NomaiFramework.Tests.Services
{
    public class BaseServiceLoaderTests
    {
        private GameObject _gameObject;
        private TestServiceLoader _serviceLoader;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject();
            _gameObject.SetActive(false);

            _serviceLoader = _gameObject.AddComponent<TestServiceLoader>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public void Awake_WhenLoadHookIsOnAwake_AddsServices()
        {
            SetBaseField("loadServicesHook", new OnAwake());

            _serviceLoader.Awake();

            Assert.AreEqual(1, _serviceLoader.AddServicesCount);
        }

        [Test]
        public void Awake_WhenLoadHookIsNotOnAwake_DoesNotAddServices()
        {
            SetBaseField("loadServicesHook", new OnStart());

            _serviceLoader.Awake();

            Assert.AreEqual(0, _serviceLoader.AddServicesCount);
        }

        [Test]
        public void OnEnable_WhenLoadHookIsOnEnable_AddsServices()
        {
            SetBaseField("loadServicesHook", new OnEnable());

            _serviceLoader.OnEnable();

            Assert.AreEqual(1, _serviceLoader.AddServicesCount);
        }

        [Test]
        public void OnEnable_WhenLoadHookIsNotOnEnable_DoesNotAddServices()
        {
            SetBaseField("loadServicesHook", new OnAwake());

            _serviceLoader.OnEnable();

            Assert.AreEqual(0, _serviceLoader.AddServicesCount);
        }

        [Test]
        public void Start_WhenLoadHookIsOnStart_AddsServices()
        {
            SetBaseField("loadServicesHook", new OnStart());

            _serviceLoader.Start();

            Assert.AreEqual(1, _serviceLoader.AddServicesCount);
        }

        [Test]
        public void Start_WhenLoadHookIsNotOnStart_DoesNotAddServices()
        {
            SetBaseField("loadServicesHook", new OnEnable());

            _serviceLoader.Start();

            Assert.AreEqual(0, _serviceLoader.AddServicesCount);
        }

        [Test]
        public void OnDisable_WhenDisposeHookIsOnDisable_RemovesServices()
        {
            SetBaseField("disposeServicesHook", new OnDisable());

            _serviceLoader.OnDisable();

            Assert.AreEqual(1, _serviceLoader.RemoveServicesCount);
        }

        [Test]
        public void OnDisable_WhenDisposeHookIsNotOnDisable_DoesNotRemoveServices()
        {
            SetBaseField("disposeServicesHook", new OnDestroy());

            _serviceLoader.OnDisable();

            Assert.AreEqual(0, _serviceLoader.RemoveServicesCount);
        }

        [Test]
        public void OnDestroy_WhenDisposeHookIsOnDestroy_RemovesServices()
        {
            SetBaseField("disposeServicesHook", new OnDestroy());

            _serviceLoader.OnDestroy();

            Assert.AreEqual(1, _serviceLoader.RemoveServicesCount);
        }

        [Test]
        public void OnDestroy_WhenDisposeHookIsNotOnDestroy_DoesNotRemoveServices()
        {
            SetBaseField("disposeServicesHook", new OnDisable());

            _serviceLoader.OnDestroy();

            Assert.AreEqual(0, _serviceLoader.RemoveServicesCount);
        }

        [Test]
        public void ActivationHook_OnlyTriggersMatchingLifecycleMessage()
        {
            SetBaseField("loadServicesHook", new OnEnable());

            _serviceLoader.Awake();
            _serviceLoader.OnEnable();
            _serviceLoader.Start();

            Assert.AreEqual(1, _serviceLoader.AddServicesCount);
        }

        [Test]
        public void DeactivationHook_OnlyTriggersMatchingLifecycleMessage()
        {
            SetBaseField("disposeServicesHook", new OnDestroy());

            _serviceLoader.OnDisable();
            _serviceLoader.OnDestroy();

            Assert.AreEqual(1, _serviceLoader.RemoveServicesCount);
        }

        private void SetBaseField(string fieldName, object value)
        {
            FieldInfo field = typeof(BaseServiceLoader).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            field?.SetValue(_serviceLoader, value);
        }
    }

    public class ServiceLoaderTests
    {
        private GameObject _gameObject;
        private ServiceLoader _serviceLoader;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Dispose();

            _gameObject = new GameObject();
            _gameObject.SetActive(false);

            _serviceLoader = _gameObject.AddComponent<ServiceLoader>();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Dispose();
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public async Task AddServices_RegistersServices()
        {
            TestService service = new();
            AnotherTestService anotherService = new();

            SetServices(service, anotherService);

            await _serviceLoader.AddServices();

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
            Assert.IsTrue(ServiceLocator.HasService<AnotherTestService>());
        }

        [Test]
        public async Task AddServices_LoadsServices()
        {
            TestService service = new();
            AnotherTestService anotherService = new();

            SetServices(service, anotherService);

            await _serviceLoader.AddServices();

            Assert.AreEqual(1, service.LoadCount);
            Assert.AreEqual(1, anotherService.LoadCount);
        }

        [Test]
        public async Task RemoveServices_RemovesServices()
        {
            TestService service = new();
            AnotherTestService anotherService = new();

            SetServices(service, anotherService);

            await _serviceLoader.AddServices();
            await _serviceLoader.RemoveServices();

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
            Assert.IsFalse(ServiceLocator.HasService<AnotherTestService>());
        }

        [Test]
        public async Task RemoveServices_DisposesServices()
        {
            TestService service = new();
            AnotherTestService anotherService = new();

            SetServices(service, anotherService);

            await _serviceLoader.AddServices();
            await _serviceLoader.RemoveServices();

            Assert.AreEqual(1, service.DisposeCount);
            Assert.AreEqual(1, anotherService.DisposeCount);
        }

        [Test]
        public void Awake_WithOnAwakeHook_RegistersServices()
        {
            TestService service = new();

            SetServices(service);
            SetBaseField(_serviceLoader, "loadServicesHook", new OnAwake());

            _serviceLoader.Awake();

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
            Assert.AreEqual(1, service.LoadCount);
        }

        [Test]
        public async Task OnDestroy_WithOnDestroyHook_RemovesServices()
        {
            TestService service = new();

            SetServices(service);
            SetBaseField(_serviceLoader, "disposeServicesHook", new OnDestroy());

            await _serviceLoader.AddServices();
            _serviceLoader.OnDestroy();

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
            Assert.AreEqual(1, service.DisposeCount);
        }

        private void SetServices(params IService[] services)
        {
            FieldInfo field = typeof(ServiceLoader).GetField(
                "services",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            field?.SetValue(_serviceLoader, services);
        }

        private static void SetBaseField(
            BaseServiceLoader loader,
            string fieldName,
            object value
        )
        {
            FieldInfo field = typeof(BaseServiceLoader).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            field?.SetValue(loader, value);
        }
    }

    public class ServiceBundleLoaderTests
    {
        private GameObject _gameObject;
        private ServiceBundleLoader _serviceLoader;
        private ServiceBundle _serviceBundle;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Dispose();

            _gameObject = new GameObject();
            _gameObject.SetActive(false);

            _serviceLoader = _gameObject.AddComponent<ServiceBundleLoader>();
            _serviceBundle = ScriptableObject.CreateInstance<ServiceBundle>();

            SetServiceBundle(_serviceBundle);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Dispose();

            Object.DestroyImmediate(_serviceBundle);
            Object.DestroyImmediate(_gameObject);
        }

        [Test]
        public async Task AddServices_RegistersBundleServices()
        {
            TestService service = new();
            AnotherTestService anotherService = new();

            _serviceBundle.services = new IService[]
            {
                service,
                anotherService
            };

            await _serviceLoader.AddServices();

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
            Assert.IsTrue(ServiceLocator.HasService<AnotherTestService>());
        }

        [Test]
        public async Task AddServices_LoadsBundleServices()
        {
            TestService service = new();
            AnotherTestService anotherService = new();

            _serviceBundle.services = new IService[]
            {
                service,
                anotherService
            };

            await _serviceLoader.AddServices();

            Assert.AreEqual(1, service.LoadCount);
            Assert.AreEqual(1, anotherService.LoadCount);
        }

        [Test]
        public async Task RemoveServices_RemovesBundleServices()
        {
            TestService service = new();
            AnotherTestService anotherService = new();

            _serviceBundle.services = new IService[]
            {
                service,
                anotherService
            };

            await _serviceLoader.AddServices();
            await _serviceLoader.RemoveServices();

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
            Assert.IsFalse(ServiceLocator.HasService<AnotherTestService>());
        }

        [Test]
        public async Task RemoveServices_DisposesBundleServices()
        {
            TestService service = new();
            AnotherTestService anotherService = new();

            _serviceBundle.services = new IService[]
            {
                service,
                anotherService
            };

            await _serviceLoader.AddServices();
            await _serviceLoader.RemoveServices();

            Assert.AreEqual(1, service.DisposeCount);
            Assert.AreEqual(1, anotherService.DisposeCount);
        }

        [Test]
        public void Awake_WithOnAwakeHook_RegistersBundleServices()
        {
            TestService service = new();

            _serviceBundle.services = new IService[]
            {
                service
            };

            SetBaseField("loadServicesHook", new OnAwake());

            _serviceLoader.Awake();

            Assert.IsTrue(ServiceLocator.HasService<TestService>());
            Assert.AreEqual(1, service.LoadCount);
        }

        [Test]
        public async Task OnDestroy_WithOnDestroyHook_RemovesBundleServices()
        {
            TestService service = new();

            _serviceBundle.services = new IService[]
            {
                service
            };

            SetBaseField("disposeServicesHook", new OnDestroy());

            await _serviceLoader.AddServices();
            _serviceLoader.OnDestroy();

            Assert.IsFalse(ServiceLocator.HasService<TestService>());
            Assert.AreEqual(1, service.DisposeCount);
        }

        private void SetServiceBundle(ServiceBundle bundle)
        {
            FieldInfo field = typeof(ServiceBundleLoader).GetField(
                "serviceBundle",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            field?.SetValue(_serviceLoader, bundle);
        }

        private void SetBaseField(string fieldName, object value)
        {
            FieldInfo field = typeof(BaseServiceLoader).GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            field?.SetValue(_serviceLoader, value);
        }
    }

    public class TestServiceLoader : BaseServiceLoader
    {
        public int AddServicesCount { get; private set; }
        public int RemoveServicesCount { get; private set; }

        public override UniTask AddServices()
        {
            AddServicesCount++;
            return UniTask.CompletedTask;
        }

        public override UniTask RemoveServices()
        {
            RemoveServicesCount++;
            return UniTask.CompletedTask;
        }
    }
}