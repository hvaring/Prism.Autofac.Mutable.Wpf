using System;
using System.Linq;
using Autofac;
using Autofac.Features.ResolveAnything;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public class AutofacContainerExtension : AutofacContainerRegistry, IAutofacContainerExtension
    {
        public IMutableContainer Instance { get; private set; }

        public AutofacContainerExtension()
        : base(new ContainerBuilder())
        {
        }

        public void FinalizeExtension()
        {
            if (Instance != null)
                return;

            Builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            Instance = new MutableContainer(Builder);
            SetLifetimeScope(Instance);
            FinalizeRegistry();
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, params (Type Type, object Instance)[] parameters)
        {
            return Instance.Resolve(type, parameters.Select(p => new TypedParameter(p.Type, p.Instance)));
        }

        public object Resolve(Type type, string name)
        {
            return Instance.ResolveNamed(name, type);
        }

        public object Resolve(Type type, string name, params (Type Type, object Instance)[] parameters)
        {
            return Instance.ResolveNamed(name, type, parameters.Select(p => new TypedParameter(p.Type, p.Instance)));
        }
    }
}