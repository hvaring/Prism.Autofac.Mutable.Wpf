using System;
using Autofac;
using Prism.Ioc;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public class AutofacContainerRegistry : IAutofacContainerRegistry
    {
        private bool _finalized;
        private ILifetimeScope _lifetimeScope;

        public AutofacContainerRegistry(ContainerBuilder builder)
        {
            Builder = builder;
        }

        public AutofacContainerRegistry(ContainerBuilder builder, ILifetimeScope lifetimeScope) 
            : this(builder)
        {
            _lifetimeScope = lifetimeScope;
            
        }

        public void FinalizeRegistry()
        {
            _finalized = true;
        }

        protected void SetLifetimeScope(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        private void CheckFinalized()
        {
            if (_finalized)
                throw new AutofacContainerRegistryFinalizedException();
        }


        public IContainerRegistry RegisterInstance(Type type, object instance)
        {
            CheckFinalized();
            Builder.RegisterInstance(instance).As(type);
            return this;
        }

        public IContainerRegistry RegisterInstance(Type type, object instance, string name)
        {
            CheckFinalized();
            Builder.RegisterInstance(instance).Named(name, type);
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to)
        {
            CheckFinalized();
            Builder.RegisterType(to).As(from).SingleInstance();
            return this;
        }

        public IContainerRegistry RegisterSingleton(Type from, Type to, string name)
        {
            CheckFinalized();
            Builder.RegisterType(to).Named(name, from);
            return this;
        }

        public IContainerRegistry Register(Type from, Type to)
        {
            CheckFinalized();
            Builder.RegisterType(to).As(from);
            return this;
        }

        public IContainerRegistry Register(Type from, Type to, string name)
        {
            CheckFinalized();
            Builder.RegisterType(to).Named(name, from);
            return this;
        }

        public bool IsRegistered(Type type)
        {
            return _lifetimeScope?.IsRegistered(type) ?? false;
        }

        public bool IsRegistered(Type type, string name)
        {
            return _lifetimeScope?.IsRegisteredWithName(name, type) ?? false;
        }

        public ContainerBuilder Builder { get; }
    }
}