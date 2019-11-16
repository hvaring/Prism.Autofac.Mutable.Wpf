# Prism.Autofac.Mutable.Wpf

This library provides [Autofac](https://github.com/autofac/Autofac) support for [Prism](https://github.com/PrismLibrary/Prism/). Support for Autofac in Prism has been removed in Prism WPF 7.0 due to Autofac's `ContainerBuilder.Update` being marked as obsolete.

The library supports both .NET Core 3.0 and .NET Framework 4.5.

## How it works

While `ContainerBuilder.Update` is obsolete, Autofac allows you to dynamically add an `IRegistrationSource` to an existing container. Adding new `IRegistrationSource`s is handled by a wrapper interface, `IMutableContainer`.
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
Your application should inherit from the abstract class `PrismApplication`, which in turn inherits from `PrismApplicationBase`.

### Module registration
Your modules will receive an `IContainerRegistry` to use when registering types. This follows the new IoC concept for Prism.

### Limitations
- The interface `IContainerExtension` gets registered by `PrismApplicationBase`. If you resolve this interface and try to register new types after the initial container has been built, a `AutofacContainerRegistryFinalizedException` is thrown. The correct way to do it (outside of a module) is to resolve `IMutableContainer` and use the `RegisterTypes` method.
- Calling `IsRegistered` will return false while building the container. Registrations are not available until the `ContainerBuilder` has been built. Modules can check `IsRegistered` for all types registered from `PrismApplication`, and for types registered by already initialized modules.