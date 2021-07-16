using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Views;
using Prism.Ioc;
using System;
using System.Windows.Media;

/**
 * @Class Name : ChannelViewerWindowViewModel.cs
 * @Description : 채널별 영상 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.04.04     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.04.04
 * @version 1.0
 */
namespace IVM.Studio.ViewModels
{
    public class ChannelViewerWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<ChannelViewerWindow>
    {
        private ImageSource displayImage;
        public ImageSource DisplayImage
        {
            get => displayImage;
            set => SetProperty(ref displayImage, value);
        }

        private ChannelViewerWindow view;

        public int Channel { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ChannelViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            EventAggregator.GetEvent<ChViewerWindowCloseEvent>().Subscribe(Close);
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(ChannelViewerWindow view)
        {
            this.view = view;
            view.Closed += WindowClosed;
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(ChannelViewerWindow view)
        {
            EventAggregator.GetEvent<ChViewerWindowCloseEvent>().Unsubscribe(Close);
        }

        /// <summary>
        /// Window 종료 이벤트
        /// </summary>
        /// <param name="type"></param>
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
            EventAggregator.GetEvent<ChViewerWindowClosedEvent>().Publish(Channel);
        }
    }
}
