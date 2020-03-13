using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Prism.Autofac.Mutable.Wpf;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

namespace DemoApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return new MainWindow();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Console.WriteLine(this.Container);
            containerRegistry.RegisterForNavigation<TestControl>();
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            {
                var viewName = viewType.FullName;
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}ViewModel, {1}", viewName, viewAssemblyName);
                return Type.GetType(viewModelName);
            });
            base.ConfigureViewModelLocator();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var testModuleType = typeof(TestModule);
            return new ModuleCatalog(new[]
                {new ModuleInfo(testModuleType.Name, testModuleType.AssemblyQualifiedName)});
        }
    }
}
