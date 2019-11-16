using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    /// <summary>
    /// Taken from <see href="https://github.com/autofac/Autofac/blob/master/src/Autofac/Core/Registration/ExternalRegistrySource.cs">here</see>.
    /// Pulls registrations from another component registry.
    /// Excludes most auto-generated registrations - currently has issues with
    /// collection registrations.
    /// 
    /// </summary>
    internal class ExternalRegistrySource : IRegistrationSource
    {
        private readonly IComponentRegistry _registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalRegistrySource "/> class.
        /// </summary>
        /// <param name="registry">Component registry to pull registrations from.</param>
        public ExternalRegistrySource(IComponentRegistry registry)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));

            _registry = registry;
        }

        /// <summary>
        /// Retrieve registrations for an unregistered service, to be used
        /// by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registrationAccessor">A function that will return existing registrations for a service.</param>
        /// <returns>Registrations providing the service.</returns>
        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            // Issue #475: This method was refactored significantly to handle
            // registrations made on the fly in parent lifetime scopes to correctly
            // pass to child lifetime scopes.

            // Issue #272: Taking from the registry the following registrations:
            //   - non-adapting own registrations: wrap them with ExternalComponentRegistration
            //   - if the registration is from the parent registry of this registry,
            //     it is already wrapped with ExternalComponentRegistration,
            //     share it as is
            return _registry.RegistrationsFor(service)
                .Where(r => r is ExternalComponentRegistration || r.Target == r)
                .Select(r =>
                    r as ExternalComponentRegistration ??

                    // equivalent to following registation builder
                    //    RegistrationBuilder.ForDelegate(r.Activator.LimitType, (c, p) => c.ResolveComponent(r, p))
                    //        .Targeting(r)
                    //        .As(service)
                    //        .ExternallyOwned()
                    //        .CreateRegistration()
                    new ExternalComponentRegistration(
                        Guid.NewGuid(),
                        new DelegateActivator(r.Activator.LimitType, (c, p) => c.ResolveComponent(r, p)),
                        new CurrentScopeLifetime(),
                        InstanceSharing.None,
                        InstanceOwnership.ExternallyOwned,
                        new[] { service },
                        r.Metadata,
                        r));
        }

        /// <summary>
        /// Gets a value indicating whether components are adapted from the same logical scope.
        /// In this case because the components that are adapted do not come from the same
        /// logical scope, we must return false to avoid duplicating them.
        /// </summary>
        public bool IsAdapterForIndividualComponents => false;

        /// <summary>
        ///  ComponentRegistration subtyped only to distinguish it from other adapted registrations.
        /// </summary>
        private class ExternalComponentRegistration : ComponentRegistration
        {
            public ExternalComponentRegistration(Guid id, IInstanceActivator activator, IComponentLifetime lifetime, InstanceSharing sharing, InstanceOwnership ownership, IEnumerable<Service> services, IDictionary<string, object> metadata, IComponentRegistration target)
                : base(id, activator, lifetime, sharing, ownership, services, metadata, target)
            {
            }
        }
    }
}