using System;
using CommonServiceLocator;
using Prism.Autofac.Mutable.Wpf.Ioc;
using Prism.Autofac.Mutable.Wpf.Modules;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity.Regions;
using AutofacCore = Autofac.Core;

namespace Prism.Autofac.Mutable.Wpf
{
    public abstract class PrismApplication : PrismApplicationBase
    {

        private AutofacContainerExtension _autofacContainerExtension;

        protected override IContainerExtension CreateContainerExtension()
        {
            _autofacContainerExtension = new AutofacContainerExtension();
            return _autofacContainerExtension;
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
        }
    }
}
