using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;

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
namespace IVM.Studio.MvvM
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
        protected IEventAggregator EventAggregator { get; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ViewModelBase(IContainerExtension container)
        {
            this.Container = container;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        /// <param name="eventAggregator"></param>
        public ViewModelBase(IContainerExtension container, IEventAggregator eventAggregator)
        {
            this.Container = Container;
            this.EventAggregator = eventAggregator;
        }
    }
}
