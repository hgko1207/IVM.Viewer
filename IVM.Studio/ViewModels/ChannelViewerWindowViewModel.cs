using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Views;
using Prism.Ioc;
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

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ChannelViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Display";

            EventAggregator.GetEvent<ChWindowClosedEvent>().Subscribe(Close);
        }

        public void OnLoaded(ChannelViewerWindow view)
        {
            this.view = view;
        }

        public void OnUnloaded(ChannelViewerWindow view)
        {
            EventAggregator.GetEvent<ChWindowClosedEvent>().Unsubscribe(Close);
        }

        private void Close(int type)
        {
            view.Close();
        }
    }
}
