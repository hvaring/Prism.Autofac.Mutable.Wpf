using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using CommonServiceLocator;

namespace Prism.Autofac.Mutable.Wpf
{
    public class AutofacServiceLocatorAdapter : ServiceLocatorImplBase
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacServiceLocatorAdapter(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }
        protected override object DoGetInstance(Type serviceType, string key)
        {
            return key != null ?
                _lifetimeScope.ResolveNamed(key, serviceType) :
                _lifetimeScope.Resolve(serviceType);
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instance = _lifetimeScope.Resolve(enumerableType);
            return ((IEnumerable)instance).Cast<object>();
        }
    }
}