using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace DemoApplication
{
    public class MainWindowViewModel : BindableBase 
    {
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            Title = "Test application";
            Loaded = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("MainRegion", nameof(TestControl));
            });
        }

        public DelegateCommand Loaded { get; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
    }
}