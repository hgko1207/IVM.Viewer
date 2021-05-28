using IVM.Studio.MvvM;
using Prism.Ioc;

namespace IVM.Studio.ViewModels
{
    public class MainViewerWindowViewModel : ViewModelBase
    {
        public MainViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Viewer";
        }
    }
}
