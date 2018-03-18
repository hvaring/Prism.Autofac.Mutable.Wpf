using System;
using System.Globalization;
using Prism.Autofac.Mutable.Wpf.Ioc;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;

namespace Prism.Autofac.Mutable.Wpf.Modules
{
    public class AutofacModuleInitializer  : IModuleInitializer
    {
        private readonly IContainerExtension _containerExtension;
        private readonly ILoggerFacade _loggerFacade;

        public AutofacModuleInitializer(IContainerExtension containerExtension, ILoggerFacade loggerFacade)
        {
            _containerExtension = containerExtension;
            _loggerFacade = loggerFacade;
        }

        public void Initialize(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            IModule moduleInstance = null;
            
            try
            {
                moduleInstance = this.CreateModule(moduleInfo);
                if (moduleInstance != null)
                {
                    //TODO: wait for WPF 7.0 to be released (not pre-version). Then uncomment code.
                    var mutableContainer = _containerExtension.GetMutableContainer();
                    mutableContainer.RegisterTypes(builder =>
                    {
                        
                        var registry = new AutofacContainerRegistry(builder);
                        //moduleInstance.RegisterTypes(registry);
                    });
                    
                    //moduleInstance.OnInitialized(_containerExtension);
                }
            }
            catch (Exception ex)
            {
                this.HandleModuleInitializationError(
                    moduleInfo,
                    moduleInstance != null ? moduleInstance.GetType().Assembly.FullName : null,
                    ex);
            }
        }


        /// <summary>
        /// Handles any exception occurred in the module Initialization process,
        /// logs the error using the <see cref="ILoggerFacade"/> and throws a <see cref="ModuleInitializeException"/>.
        /// This method can be overridden to provide a different behavior.
        /// </summary>
        /// <param name="moduleInfo">The module metadata where the error happenened.</param>
        /// <param name="assemblyName">The assembly name.</param>
        /// <param name="exception">The exception thrown that is the cause of the current error.</param>
        /// <exception cref="ModuleInitializeException"></exception>
        public virtual void HandleModuleInitializationError(ModuleInfo moduleInfo, string assemblyName, Exception exception)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            Exception moduleException;

            if (exception is ModuleInitializeException)
            {
                moduleException = exception;
            }
            else
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    moduleException = new ModuleInitializeException(moduleInfo.ModuleName, assemblyName, exception.Message, exception);
                }
                else
                {
                    moduleException = new ModuleInitializeException(moduleInfo.ModuleName, exception.Message, exception);
                }
            }

            this._loggerFacade.Log(moduleException.ToString(), Category.Exception, Priority.High);

            throw moduleException;
        }

        /// <summary>
        /// Uses the container to resolve a new <see cref="IModule"/> by specifying its <see cref="Type"/>.
        /// </summary>
        /// <param name="moduleInfo">The module to create.</param>
        /// <returns>A new instance of the module specified by <paramref name="moduleInfo"/>.</returns>
        protected virtual IModule CreateModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null)
                throw new ArgumentNullException(nameof(moduleInfo));

            return this.CreateModule(moduleInfo.ModuleType);
        }

        /// <summary>
        /// Uses the container to resolve a new <see cref="IModule"/> by specifying its <see cref="Type"/>.
        /// </summary>
        /// <param name="typeName">The type name to resolve. This type must implement <see cref="IModule"/>.</param>
        /// <returns>A new instance of <paramref name="typeName"/>.</returns>
        protected virtual IModule CreateModule(string typeName)
        {
            Type moduleType = Type.GetType(typeName);
            if (moduleType == null)
            {
                //TODO: fix
                //throw new ModuleInitializeException(string.Format(CultureInfo.CurrentCulture, Prism.Properties.Resources.FailedToGetType, typeName));
            }

            return (IModule)_containerExtension.Resolve(moduleType);
        }
    }
}