using System;
using System.Windows;
using CommonServiceLocator;
using FluentAssertions;
using NUnit.Framework;
using Prism.Autofac.Mutable.Wpf.Ioc;
using Prism.Autofac.Mutable.Wpf.Modules;
using Prism.Autofac.Mutable.Wpf.Regions;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Prism.Autofac.Mutable.Wpf.Tests
{
    [TestFixture]
    public class PrismApplicationFixture
    {
        private TestPrismApplication _app;

        [OneTimeSetUp]
        public void Initialize()
        {
            _app = new TestPrismApplication();
            _app.Initialize();
        }


        [Test]
        public void PrismApplicationShouldRegisterCustomAutofacComponents()
        {
            _app.Container.Resolve<IServiceLocator>().Should().BeOfType<AutofacServiceLocatorAdapter>();
            _app.Container.Resolve<IRegionNavigationContentLoader>().Should().BeOfType<AutofacRegionNavigationContentLoader>();
            _app.Container.Resolve<IModuleInitializer>().Should().BeOfType<AutofacModuleInitializer>();
        }

        [Test]
        public void PrismApplicationShouldRegisterTypesFromModules()
        {
            _app.Container.Resolve<ITestService>().Should().NotBeNull();
        }

        [Test]
        public void PrismApplicationShouldInitializeModules()
        {
            _app.Container.Resolve<ITestService>().ModuleInitialized.Should().BeTrue();
        }

        [Test]
        public void ContainerExtensionShouldThrowWhenRegisteringTypesAfterInitialization()
        {
            Action action = () => _app.Container.Resolve<IContainerExtension>().Register<TestService>();
            action.Should().Throw<AutofacContainerRegistryFinalizedException>();
        }
    }

    internal class TestService : ITestService
    {
        public bool ModuleInitialized { get; set; } = false;
    }

    internal interface ITestService
    {
        bool ModuleInitialized { get; set; }
    }

    internal class TestModule : IModule
    {

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ITestService, TestService>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            containerProvider.Resolve<ITestService>().ModuleInitialized = true;
        }
    }

    internal class TestPrismApplication : PrismApplication
    {
        protected override IModuleCatalog CreateModuleCatalog()
        {
            var testModuleType = typeof(TestModule);
            return new ModuleCatalog(new ModuleInfo[] {new ModuleInfo(testModuleType.Name, testModuleType.AssemblyQualifiedName)});
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            InitializeModules();
        }

        protected override Window CreateShell()
        {
            return null;
        }

        protected override void OnInitialized()
        {
        }
    }
    
}