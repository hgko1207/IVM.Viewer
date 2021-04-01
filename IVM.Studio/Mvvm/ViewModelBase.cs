using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;

namespace IVM.Studio.MvvM
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
        protected IContainerExtension Container { get; }

        public ViewModelBase(IContainerExtension container)
        {
            this.Container = Container;
        }

        public ViewModelBase(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.Container = Container;
            this.EventAggregator = eventAggregator;
        }
    }
}
