using System;
using Autofac;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public class AutofacContainerRegistry : IAutofacContainerRegistry
    {
        private bool _finalized;

        public void FinalizeRegistry()
        {
            _finalized = true;
        }

        private void CheckFinalized()
        {
            if (_finalized)
                throw new AutofacContainerRegistryFinalizedException();
        }

        public AutofacContainerRegistry(ContainerBuilder builder)
        {
            Builder = builder;
        }
        public void RegisterInstance(Type type, object instance)
        {
            CheckFinalized();
            Builder.RegisterInstance(instance).As(type);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            CheckFinalized();
            Builder.RegisterType(to).As(from).SingleInstance();
        }

        public void Register(Type from, Type to)
        {
            CheckFinalized();
            Builder.RegisterType(to).As(from);
        }

        public void Register(Type from, Type to, string name)
        {
            CheckFinalized();
            Builder.RegisterType(to).Named(name, from);
        }

        public ContainerBuilder Builder { get; }
    }
}