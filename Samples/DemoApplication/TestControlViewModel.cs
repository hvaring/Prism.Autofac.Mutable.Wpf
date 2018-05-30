using Prism.Mvvm;
using Prism.Regions;

namespace DemoApplication
{
    public class TestControlViewModel : BindableBase, INavigationAware
    {
        private readonly ITestService _testService;

        public TestControlViewModel(ITestService testService)
        {
            _testService = testService;
        }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Number = _testService.Number;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private int _number;
        public int Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }
    }
}