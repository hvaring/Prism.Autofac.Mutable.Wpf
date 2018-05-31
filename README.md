# Prism.Autofac.Mutable.Wpf

This library offers an alternative to the official Prism.Autofac library for WPF that supports modules. Module support for Autofac in Prism has been removed in Prism WPF 7.0 due to Autofac's `ContainerBuilder.Update` being marked as obsolete.

## How it works

While `ContainerBuilder.Update` is obsolete, Autofac allows you to dynamically add an `IRegistrationSource` to an existing container. This is also one of the workarounds mentioned in [Autofac ContainerBuilder.Update Obsolete](https://github.com/PrismLibrary/Prism/issues/969#issuecomment-291617882). Adding new `IRegistrationSource`s is handled by a wrapper interface, `IMutableConctainer`.
```
public interface IMutableContainer : IContainer
{
    void RegisterTypes(Action<ContainerBuilder> builder);
}
```
Internally, the method `RegisterTypes` creates a new container with all the new registrations, and adds itself as an `ExternalRegistrySource` to the existing container. `IMutableContainer` is registered as `IContainer`.

The `AutofacRegionNavigationContentLoader` and `AutofacServiceLocatorAdapter` are based completely on the classes by the same name in `Prism.Autofac`, although they take an `ILifetimeScope` instead of an `IContainer` as input.

### Loading modules
A custom `IModuleInitializer` (`AutofacModuleInitializer`) is registered to resolve modules. For each module, it calls `IMutableContainer`'s `RegisterTypes`.
```
mutableContainer.RegisterTypes(builder =>
{
    var registry = new AutofacContainerRegistry(builder);
    moduleInstance.RegisterTypes(registry);
    registry.FinalizeRegistry();
});
```
The `AutofacContainerRegistry` is an `IContainerRegistry`, which is what `IModule` expects.

## How to use it
Your application should inherit from the abstract class `PrismApplication`, which in turn inherits from `PrismApplicationBase`. See [[XF] Container abstractions - PrismApplication restructure (Breaking) #1288](https://github.com/PrismLibrary/Prism/pull/1288) for more information about that.

### Module registration
Your modules will receive an `IContainerRegistry` to use when registering types. This follows the new IoC concept for Prism.

### Limitations
The interface `IContainerExtension` gets registered by `PrismApplicationBase`. If you resolve this interface and try to register new types after the initial container has been built, a `AutofacContainerRegistryFinalizedException` is thrown. The correct way to do it (outside of a module) is to resolve `IMutableContainer` and use the `RegisterTypes` method.
