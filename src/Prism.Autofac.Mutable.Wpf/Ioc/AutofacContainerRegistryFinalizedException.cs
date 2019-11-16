using System;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public class AutofacContainerRegistryFinalizedException : ApplicationException
    {
        public override string Message => $"The {nameof(AutofacContainerRegistry)} has already been built. To register types, please use the {nameof(IMutableContainer)} interface.";
    }
}