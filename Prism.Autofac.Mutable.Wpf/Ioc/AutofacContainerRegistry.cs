using System;
using Autofac;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public class AutofacContainerRegistry : IAutofacContainerRegistry
    {
        public AutofacContainerRegistry(ContainerBuilder builder)
        {
            Builder = builder;
        }
        public void RegisterInstance(Type type, object instance)
        {
            Builder.RegisterInstance(instance).As(type);
        }

        public void RegisterSingleton(Type from, Type to)
        {
            Builder.RegisterType(to).As(from).SingleInstance();
        }

        public void Register(Type from, Type to)
        {
            Builder.RegisterType(to).As(from);
        }

        public void Register(Type from, Type to, string name)
        {
            Builder.RegisterType(to).Named(name, from);
        }

        public ContainerBuilder Builder { get; }
    }
}