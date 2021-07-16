using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Windows.Media;

/**
 * @Class Name : ChannelHistogramWindowViewModel.cs
 * @Description : 채널별 히스토그램 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.27     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.06.27
 * @version 1.0
 */
namespace IVM.Studio.ViewModels
{
    public class ChannelHistogramWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<ChannelHistogramWindow>
    {
        private ImageSource histogramImage;
        public ImageSource HistogramImage
        {
            get => histogramImage;
            set => SetProperty(ref histogramImage, value);
        }

        public ChannelType ChannelType { get; set; }

        private ChannelHistogramWindow view;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ChannelHistogramWindowViewModel(IContainerExtension container) : base(container)
        {
            EventAggregator.GetEvent<RefreshChHistogramEvent>().Subscribe(RefreshHistogram, ThreadOption.UIThread, false, type => type == ChannelType);
            EventAggregator.GetEvent<ChHistogramWindowCloseEvent>().Subscribe(Close);

            RefreshHistogram(ChannelType);
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(ChannelHistogramWindow view)
        {
            this.view = view;
            view.Closed += WindowClosed;
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(ChannelHistogramWindow view)
        {
            EventAggregator.GetEvent<RefreshChHistogramEvent>().Unsubscribe(RefreshHistogram);
            EventAggregator.GetEvent<ChHistogramWindowCloseEvent>().Unsubscribe(Close);
        }

        /// <summary>
        /// 새로고침 이벤트
        /// </summary>
        private void RefreshHistogram(ChannelType type)
        {
            HistogramImage = Container.Resolve<DataManager>().ColorChannelInfoMap[type].HistogramImage;
        }

        /// <summary>
        /// Window 종료 이벤트
        /// </summary>
        private void Close(int type)
        {
            view.Close();
        }

        /// <summary>
        /// Window 종료 시킬 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosed(object sender, EventArgs e)
        {
            EventAggregator.GetEvent<ChHistogramWindowClosedEvent>().Publish(ChannelType);
        }
    }
}
