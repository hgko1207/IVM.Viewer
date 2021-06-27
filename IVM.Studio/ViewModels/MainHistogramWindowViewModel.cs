using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Ioc;
using System;
using Drawing = System.Windows.Media;

/**
* @Class Name : MainHistogramWindowViewModel.cs
* @Description : 메인 히스토그램 뷰어 뷰 모델
* @
* @ 수정일         수정자              수정내용
* @ ----------   ---------   -------------------------------
* @ 2021.06.06     고형균              최초생성
*
* @author 고형균
* @since 2021.06.06
* @version 1.0
*/
namespace IVM.Studio.ViewModels
{
    public class MainHistogramWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<MainHistogramWindow>
    {
        private Drawing.ImageSource histogramImage;
        public Drawing.ImageSource HistogramImage
        {
            get => histogramImage;
            set => SetProperty(ref histogramImage, value);
        }

        private MainHistogramWindow view;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public MainHistogramWindowViewModel(IContainerExtension container) : base(container)
        {
            EventAggregator.GetEvent<HistogramCloseEvent>().Subscribe(() => view.Close());

            Refresh();
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(MainHistogramWindow view)
        {
            this.view = view;
            view.Closed += WindowClosed;
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(MainHistogramWindow view)
        {
            EventAggregator.GetEvent<HistogramCloseEvent>().Unsubscribe(() => view.Close());
        }

        /// <summary>
        /// 새로고침
        /// </summary>
        private void Refresh()
        {
            HistogramImage = Container.Resolve<DataManager>().HistogramImage;
        }

        /// <summary>
        /// Window 종료 시킬 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosed(object sender, EventArgs e)
        {
            EventAggregator.GetEvent<HistogramClosedEvent>().Publish();
        }
    }
}
