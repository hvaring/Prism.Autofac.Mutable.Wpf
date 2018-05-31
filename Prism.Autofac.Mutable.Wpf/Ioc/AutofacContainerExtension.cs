using System;
using Autofac;
using Autofac.Features.ResolveAnything;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public class AutofacContainerExtension : AutofacContainerRegistry, IAutofacContainerExtension
    {
        public bool SupportsModules => true;
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
            FinalizeRegistry();
        }

        public object Resolve(Type type)
        {
            return Instance.Resolve(type);
        }

        public object Resolve(Type type, string name)
        {
            return Instance.ResolveNamed(name, type);
        }

        public object ResolveViewModelForView(object view, Type viewModelType)
        {
            return Instance.Resolve(viewModelType);
        }
    }
}