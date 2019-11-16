using Autofac;
using Prism.Ioc;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public interface IAutofacContainerRegistry : IContainerRegistry
    {
        /// <summary>
        /// Gets the <see cref="ContainerBuilder"/> used to register the services for the application
        /// </summary>
        ContainerBuilder Builder { get; }
    }
}