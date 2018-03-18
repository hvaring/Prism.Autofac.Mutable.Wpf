# Prism.Autofac.Mutable.Wpf

This library offers an alternative to the official Prism.Autofac library for WPF that supports modules. Module support for Autofac in Prism will be removed in Prism WPF 7.0 due to Autofac's `ContainerBuilder.Update` being marked as obsolete.

## How it works

While `ContainerBuilder.Update` is obsolete, Autofac allows you to create child scopes and include any new registrations. This is also one of the workarounds mentioned in [Discussion: ContainerBuilder.Update Marked Obsolete](https://github.com/autofac/Autofac/issues/811). Tracking and returning the latest lifetime scope is handled by a wrapper interface, `IMutableConctainer`.
```
public interface IMutableContainer : IContainer
{
    void RegisterTypes(Action<ContainerBuilder> builder);
}
```
Internally, the method `RegisterTypes` creates a new child scope and routes all container-requests to the new scope. `IMutableContainer` is registered as `IComponentContext`, `IContainer` and `ILifetimeScope`.
