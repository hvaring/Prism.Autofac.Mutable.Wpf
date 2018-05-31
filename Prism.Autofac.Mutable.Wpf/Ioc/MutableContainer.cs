using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Core.Resolving;
using Autofac.Util;

namespace Prism.Autofac.Mutable.Wpf.Ioc
{
    internal class MutableContainer : Disposable, IMutableContainer, IServiceProvider
    {
        private readonly IContainer _container;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly List<IContainer> _additionalRegistrations;

        internal MutableContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(this).As<IMutableContainer>().As<IContainer>().SingleInstance();
            _container = builder.Build();
            _additionalRegistrations = new List<IContainer>();
            _lifetimeScope = _container.Resolve<ILifetimeScope>();
            _lifetimeScope.ChildLifetimeScopeBeginning += OnChildLifetimeScopeBeginning;
            _lifetimeScope.CurrentScopeEnding += OnCurrentScopeEnding;
            _lifetimeScope.ResolveOperationBeginning += OnResolveOperationBeginning;
        }

        /// <summary>
        /// Registers additional types to be made available in the container.
        /// </summary>
        /// <param name="configurationAction">Configuration for the <see cref="ContainerBuilder"/></param>
        public void RegisterTypes(Action<ContainerBuilder> configurationAction)
        {
            if (configurationAction == null)
                throw new ArgumentNullException(nameof(configurationAction));

            var builder = new ContainerBuilder();
            configurationAction(builder);
            var container = builder.Build();
            _additionalRegistrations.Add(container);

            // We need to re-add components back to the root scope.
            // This is important so that the new registrations are available, 
            // for example when resolving singletons that have not yet been resolved.
            // Without this, singletons would not get any of the registrations done with RegisterTypes, 
            // even if they were resolved after additional registrations.
            var registrationSource = new ExternalRegistrySource(container.ComponentRegistry);
            _lifetimeScope.ComponentRegistry.AddRegistrationSource(registrationSource);
        }

        private void OnResolveOperationBeginning(object sender, ResolveOperationBeginningEventArgs e)
        {
            ResolveOperationBeginning?.Invoke(sender, e);
        }

        private void OnCurrentScopeEnding(object sender, LifetimeScopeEndingEventArgs e)
        {
            CurrentScopeEnding?.Invoke(sender, e);
        }

        private void OnChildLifetimeScopeBeginning(object sender, LifetimeScopeBeginningEventArgs e)
        {
            ChildLifetimeScopeBeginning?.Invoke(sender, e);
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return _lifetimeScope.BeginLifetimeScope();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return _lifetimeScope.BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return _lifetimeScope.BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return _lifetimeScope.BeginLifetimeScope(tag, configurationAction);
        }

        public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return _lifetimeScope.ResolveComponent(registration, parameters);
        }

        public IComponentRegistry ComponentRegistry => _lifetimeScope.ComponentRegistry;



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _container.Dispose();
                _additionalRegistrations.ForEach(s => s.Dispose());
            }
            base.Dispose(disposing);
        }

        public IDisposer Disposer => _lifetimeScope.Disposer;
        public object Tag => _lifetimeScope.Tag;
        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;
        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding;
        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning;

        public object GetService(Type serviceType)
        {
            return ((IServiceProvider)_lifetimeScope).GetService(serviceType);
        }
    }
}