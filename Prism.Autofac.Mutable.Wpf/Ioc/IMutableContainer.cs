using System;
using Autofac;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    public interface IMutableContainer : IContainer
    {
        void RegisterTypes(Action<ContainerBuilder> builder);
    }
}