using System;
using CommonServiceLocator;
using Prism.Autofac.Mutable.Wpf.Ioc;
using Prism.Autofac.Mutable.Wpf.Modules;
using Prism.Autofac.Mutable.Wpf.Regions;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using AutofacCore = Autofac.Core;

namespace Prism.Autofac.Mutable.Wpf
{
    public abstract class PrismApplication : PrismApplicationBase
    {
        protected override IContainerExtension CreateContainerExtension()
        {
            return new AutofacContainerExtension();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            base.RegisterRequiredTypes(containerRegistry);
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, AutofacRegionNavigationContentLoader>();
            containerRegistry.RegisterSingleton<IServiceLocator, AutofacServiceLocatorAdapter>();
            containerRegistry.RegisterSingleton<IModuleInitializer, AutofacModuleInitializer>();
        }

        protected override void RegisterFrameworkExceptionTypes()
        {
            base.RegisterFrameworkExceptionTypes();
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacCore.DependencyResolutionException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacCore.Registration.ComponentNotRegisteredException));
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(AutofacContainerRegistryFinalizedException));
        }
    }
}
