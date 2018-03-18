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
        private readonly List<ILifetimeScope> _managedChildScopes;

        private ILifetimeScope _currentLifetimeScope;

        internal MutableContainer(IContainer container)
        {
            _container = container;
            _managedChildScopes = new List<ILifetimeScope>();
            _currentLifetimeScope = container.Resolve<ILifetimeScope>();
            //Initialize child scope and register self.
            RegisterTypes(b => b.RegisterInstance(this)
                .As<IMutableContainer>()
                .As<IContainer>());
        }

        public void RegisterTypes(Action<ContainerBuilder> configurationAction)
        {
            if (configurationAction == null)
                throw new ArgumentNullException(nameof(configurationAction));

            _currentLifetimeScope.ChildLifetimeScopeBeginning -= OnChildLifetimeScopeBeginning;
            _currentLifetimeScope.CurrentScopeEnding -= OnCurrentScopeEnding;
            _currentLifetimeScope.ResolveOperationBeginning -= OnResolveOperationBeginning;
            _currentLifetimeScope = _currentLifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(this).As<ILifetimeScope>().As<IComponentContext>();
                configurationAction(builder);
            });
            _managedChildScopes.Add(_currentLifetimeScope);

            _currentLifetimeScope.ChildLifetimeScopeBeginning += OnChildLifetimeScopeBeginning;
            _currentLifetimeScope.CurrentScopeEnding += OnCurrentScopeEnding;
            _currentLifetimeScope.ResolveOperationBeginning += OnResolveOperationBeginning;
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
            return _currentLifetimeScope.BeginLifetimeScope();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return _currentLifetimeScope.BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return _currentLifetimeScope.BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return _currentLifetimeScope.BeginLifetimeScope(tag, configurationAction);
        }
        
        public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return _currentLifetimeScope.ResolveComponent(registration, parameters);
        }

        public IComponentRegistry ComponentRegistry => _currentLifetimeScope.ComponentRegistry;

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _container.Dispose();
                _managedChildScopes.ForEach(s => s.Dispose());
            }
            base.Dispose(disposing);
        }

        public IDisposer Disposer => _currentLifetimeScope.Disposer;
        public object Tag => _currentLifetimeScope.Tag;
        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;
        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding;
        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning;

        public object GetService(Type serviceType)
        {
            return ((IServiceProvider)_currentLifetimeScope).GetService(serviceType);
        }
    }
}