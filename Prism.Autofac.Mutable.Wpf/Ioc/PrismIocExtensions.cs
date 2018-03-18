using Autofac;
using Prism.Ioc;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public static class PrismIocExtensions
    {
        public static ContainerBuilder GetBuilder(this IContainerRegistry registry)
        {
            return ((IAutofacContainerRegistry)registry).Builder;
        }

        public static IMutableContainer GetMutableContainer(this IContainerExtension containerExtension)
        {
            return ((IContainerExtension<IMutableContainer>) containerExtension).Instance;
        }
    }
}