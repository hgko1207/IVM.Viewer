using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows.Threading;
using Unity;

/**
 * @Class Name : ViewModelBase.cs
 * @Description : 뷰 모델 공통
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.03.29     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.03.29
 * @version 1.0
 */
namespace IVM.Studio.Mvvm
{
    public abstract class ViewModelBase : BindableBase
    {
        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        protected IContainerExtension Container { get; }

        private IEventAggregator eventAggregator;
        /// <summary>
        /// The EventAggregator
        /// </summary>
        public IEventAggregator EventAggregator
        {
            get { return eventAggregator; }
            private set { this.SetProperty<IEventAggregator>(ref this.eventAggregator, value); }
        }

        private IRegionManager regionManager;
        /// <summary>
        /// The region manager
        /// </summary>
        public IRegionManager RegionManager
        {
            get { return regionManager; }
            private set { this.SetProperty<IRegionManager>(ref this.regionManager, value); }
        }

        public Dispatcher Dispatcher { get; set; }
        protected virtual void Invoke(Action action) => Dispatcher.Invoke(action);

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ViewModelBase(IContainerExtension container)
        {
            this.Container = container;
            EventAggregator = container.Resolve<IEventAggregator>();
            RegionManager = container.Resolve<IRegionManager>();
        }
    }
}
