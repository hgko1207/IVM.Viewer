using Prism.Events;
using Prism.Mvvm;
using Unity;

namespace IVM.Viewer.MvvM
{
    public abstract class ViewModelBase : BindableBase
    {
        private string title;
        public string TreTitleItemList
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        protected IEventAggregator EventAggregator { get; }
        protected IUnityContainer Container { get; }

        public ViewModelBase(IUnityContainer container)
        {
            this.Container = Container;
        }

        public ViewModelBase(IUnityContainer container, IEventAggregator eventAggregator)
        {
            this.Container = Container;
            this.EventAggregator = eventAggregator;
        }
    }
}
