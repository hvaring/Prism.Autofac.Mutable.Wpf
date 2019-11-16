using System;
using System.Collections.Generic;
using System.Linq;
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
            builder.RegisterType<Singleton>().SingleInstance();
            builder.RegisterType<FuncSingleton>().SingleInstance();
            _mutable = new MutableContainer(builder);
        }

        [Test]
        public void MutableContainerShouldBeRegisteredAsContainer()
        {
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

        [Test]
        public void SingletonShouldGetAllRegistrationsWhenResolvedAfterNewRegistrations()
        {
            _mutable.RegisterTypes(builder =>
            {
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
            });
            var singleton = _mutable.Resolve<Singleton>();
            singleton.Registrations.Count().Should().Be(3);
        }

        [Test]
        public void SingletonShouldNotGetAllRegistrationsWhenResolvedBeforeNewRegistrations()
        {
            var singleton = _mutable.Resolve<Singleton>();
            singleton.Registrations.Count().Should().Be(0);
            _mutable.RegisterTypes(builder =>
            {
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
            });
            singleton = _mutable.Resolve<Singleton>();
            singleton.Registrations.Count().Should().Be(0);
        }

        [Test]
        public void LifetimeScopesResolvedInNewRegistrationsShouldReturnOriginalScope()
        {
            var scope = _mutable.Resolve<ILifetimeScope>();
            _mutable.RegisterTypes(builder => builder.RegisterType<ClassWithLifetimeScope>());
            var classWithScope = _mutable.Resolve<ClassWithLifetimeScope>();
            classWithScope.Scope.Should().Be(scope);
        }

        [Test]
        public void TypesInNewRegistrationsShouldBeAbleToResolveOriginalComponents()
        {
            var singleton = _mutable.Resolve<Singleton>();
            _mutable.RegisterTypes(builder => builder.RegisterType<ClassWithSingleton>());
            var classWithSingleton = _mutable.Resolve<ClassWithSingleton>();
            classWithSingleton.Singleton.Should().Be(singleton);
        }

        [Test]
        public void FuncsInSingletonsShouldReturnNewRegistrations()
        {
            var funcSingleton = _mutable.Resolve<FuncSingleton>();
            funcSingleton.Registrations.Count().Should().Be(0);
            _mutable.RegisterTypes(builder =>
            {
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
                builder.RegisterType<NewClassRegistration>().As<INewClassRegistration>();
            });
            funcSingleton.Registrations.Count().Should().Be(3);
        }

        private class ClassWithSingleton
        {
            public Singleton Singleton { get; }

            public ClassWithSingleton(Singleton singleton)
            {
                Singleton = singleton;
            }
        }

        private class FuncSingleton
        {
            private readonly Func<IEnumerable<INewClassRegistration>> _registrationFactory;
            public IEnumerable<INewClassRegistration> Registrations => _registrationFactory();

            public FuncSingleton(Func<IEnumerable<INewClassRegistration>> registrationFactory)
            {
                _registrationFactory = registrationFactory;
            }
        }

        private class ClassWithLifetimeScope
        {
            public ILifetimeScope Scope { get; }

            public ClassWithLifetimeScope(ILifetimeScope scope)
            {
                Scope = scope;
            }
        }

        private class Singleton
        {
            public IEnumerable<INewClassRegistration> Registrations { get; }

            public Singleton(IEnumerable<INewClassRegistration> registrations)
            {
                Registrations = registrations;
            }
        }

        private class NewClassRegistration : INewClassRegistration
        {

        }

        private interface INewClassRegistration
        {

        }
    }
}
