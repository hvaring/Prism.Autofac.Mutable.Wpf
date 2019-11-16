using Autofac;
using FluentAssertions;
using NUnit.Framework;
using Prism.Autofac.Mutable.Wpf.Ioc;
using Prism.Ioc;

namespace Prism.Autofac.Mutable.Wpf.Tests
{
    [TestFixture]
    public class ExtensionAndRegistryFixture
    {
        [Test]
        public void AutofacContainerRegistryShouldReturnFalseForIsRegisteredDuringRegistration()
        {
            var registry = new AutofacContainerRegistry(new ContainerBuilder());
            registry.Register<IService, Service>();
            registry.IsRegistered<IService>().Should().BeFalse();
        }

        [Test]
        public void AutofacContainerRegistryShouldReturnTrueForIsRegisteredAfterRegistration()
        {
            var container = new MutableContainer(new ContainerBuilder());

            AutofacContainerRegistry registry = null;
            container.RegisterTypes(b =>
            {
                registry = new AutofacContainerRegistry(b, container);
                registry.Register<IService, Service>();
                registry.FinalizeRegistry();
                
            });
            registry.IsRegistered<IService>().Should().BeTrue();
        }

        [Test]
        public void AutofacContainerRegistryShouldReturnTrueForIsRegisteredOnExistingRegistrations()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Service>().As<IService>();
            var container = new MutableContainer(builder);

            var registry = new AutofacContainerRegistry(new ContainerBuilder(), container);
            registry.IsRegistered<IService>().Should().BeTrue();
        }


        private interface IService
        {
        }

        private class Service : IService
        {
        }
    }
}