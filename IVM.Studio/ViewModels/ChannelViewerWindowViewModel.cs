using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Commands;
using Prism.Ioc;
using System.Windows.Input;
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

        public ICommand ClosedCommand { get; private set; }

        private ChannelViewerWindow view;

        public int Channel { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ChannelViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            ClosedCommand = new DelegateCommand(WindowClosed);

            EventAggregator.GetEvent<ChWindowCloseEvent>().Subscribe(Close);
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(ChannelViewerWindow view)
        {
            this.view = view;
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(ChannelViewerWindow view)
        {
            EventAggregator.GetEvent<ChWindowCloseEvent>().Unsubscribe(Close);
        }

        /// <summary>
        /// Window 종료 이벤트
        /// </summary>
        private void WindowClosed()
        {
            EventAggregator.GetEvent<ChWindowClosedEvent>().Publish(Channel);
        }

        /// <summary>
        /// Window 종료 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void Close(int type)
        {
            view.Close();
        }
    }
}
