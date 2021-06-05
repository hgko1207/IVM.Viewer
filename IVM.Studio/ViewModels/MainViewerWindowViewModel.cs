using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.MvvM;
using IVM.Studio.Views;
using Prism.Ioc;

namespace IVM.Studio.ViewModels
{
    public class MainViewerWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<MainViewerWindow>
    {
        private MainViewerWindow view;

        public MainViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Viewer";

            EventAggregator.GetEvent<MainViewerCloseEvent>().Subscribe(Close);
        }

        public void OnLoaded(MainViewerWindow view)
        {
            this.view = view;
        }

        public void OnUnloaded(MainViewerWindow view)
        {
            EventAggregator.GetEvent<MainViewerCloseEvent>().Unsubscribe(Close);
        }

        private void Close()
        {
            view.Close();
        }
    }
}
