using Prism.Ioc;
using Prism.Modularity;

namespace DemoApplication
{
    public class TestModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

            containerRegistry.Register<ITestService, TestService>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
        }
    }
}