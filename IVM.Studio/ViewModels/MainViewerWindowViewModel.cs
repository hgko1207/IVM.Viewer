using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using IVM.Studio.Views.UserControls;
using Prism.Ioc;
using System;
using System.Windows;
using System.Windows.Controls;

/**
 * @Class Name : MainViewerWindowViewModel.cs
 * @Description : 메인 뷰어 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.16     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.06.16
 * @version 1.0
 */
namespace IVM.Studio.ViewModels
{
    public class MainViewerWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<MainViewerWindow>
    {
        /// <summary>뷰어 화면에 표현할 페이지입니다. 동영상 또는 이미지 페이지입니다.</summary>
        private UserControl viewerPage;
        public UserControl ViewerPage
        {
            get => viewerPage;
            set => SetProperty(ref viewerPage, value);
        }

        private MainViewerWindow window;

        private UserControl imagePage;
        private UserControl videoPage;

        private DataManager dataManager;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public MainViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Image Viewer";

            dataManager = Container.Resolve<DataManager>();
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(MainViewerWindow view)
        {
            this.window = view;
            window.Closed += WindowClosed;
            window.Activated += WindowActivated;
            window.Deactivated += WindowDeactivated;

            EventAggregator.GetEvent<ViewerPageChangedEvent>().Subscribe(ViewerChanged);
            EventAggregator.GetEvent<MainViewerCloseEvent>().Subscribe(() => window.Close());

            imagePage = new ImageViewer(EventAggregator) { WindowId = view.WindowId };
            videoPage = new VideoViewer() { WindowId = view.WindowId };

            ViewerChanged();

            dataManager.MainViewerOpend = true;
            EventAggregator.GetEvent<MainViewerOpendEvent>().Publish();
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(MainViewerWindow view)
        {
            EventAggregator.GetEvent<ViewerPageChangedEvent>().Unsubscribe(ViewerChanged);
            EventAggregator.GetEvent<MainViewerCloseEvent>().Unsubscribe(() => window.Close());
        }

        /// <summary>
        /// 뷰어 변경
        /// </summary>
        private void ViewerChanged()
        {
            string viewerName = Container.Resolve<DataManager>().ViewerName;
            if (viewerName == nameof(VideoViewer))
            {
                Title = "Video Viewer";
                ViewerPage = videoPage;
            }
            else
            {
                Title = "Image Viewer - #" + window.WindowId;
                ViewerPage = imagePage;
            }
        }

        /// <summary>
        /// 창이 활성화 상태일 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowActivated(object sender, EventArgs e)
        {
            if (window.IsActive)
            {
                dataManager.MainWindowId = window.WindowId;
            }
        }

        /// <summary>
        /// 창이 비활성화 상태 일 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowDeactivated(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Window 종료 시킬 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosed(object sender, EventArgs e)
        {
            dataManager.MainViewerOpend = false;
            EventAggregator.GetEvent<MainViewerClosedEvent>().Publish();
        }
    }
}
