using System;
using Autofac;
using FluentAssertions;
using NUnit.Framework;
using Prism.Autofac.Mutable.Wpf.Ioc;

namespace Prism.Autofac.Mutable.Wpf.Tests
{
    [TestFixture]
    public class MutableContainerFixture
    {
        private IMutableContainer _mutable;
        [SetUp]
        public void Initialize()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            _mutable = new MutableContainer(container);
        }


        [Test]
        public void MutableContainerShouldBeRegisteredAsAutofacComponents()
        {

            _mutable.Resolve<ILifetimeScope>().Should().Be(_mutable);
            _mutable.Resolve<IComponentContext>().Should().Be(_mutable);
            _mutable.Resolve<IContainer>().Should().Be(_mutable);
        }

        [Test]
        public void MutableContainerShouldBeRegisteredAsSelf()
        {
            _mutable.Resolve<IMutableContainer>().Should().Be(_mutable);
        }

        [Test]
        public void MutableContainerShouldRetainSelfRegistrationsOnRegisterTypes()
        {
            _mutable.RegisterTypes(b => {});
            _mutable.Resolve<ILifetimeScope>().Should().Be(_mutable);
            _mutable.Resolve<IComponentContext>().Should().Be(_mutable);
            _mutable.Resolve<IContainer>().Should().Be(_mutable);
        }

        [Test]
        public void MutableContainerRegisterTypesShouldRegisterTypes()
        {
            _mutable.RegisterTypes(builder => builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>());
            _mutable.Resolve<INewClassRegistration>().Should().BeAssignableTo<NewClassRegistration>();
        }

        [Test]
        public void MutableContainerShouldDisposeAllChildLifetimeScopes()
        {
            _mutable.RegisterTypes(builder => builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>());
            _mutable.Dispose();
            Action resolveAction = () => _mutable.Resolve<INewClassRegistration>();
            resolveAction.Should().Throw<ObjectDisposedException>();
        }

        private class NewClassRegistration : INewClassRegistration
        {

        }

        private interface INewClassRegistration
        {

        }
    }
}
