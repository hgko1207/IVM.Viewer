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
            set
            {
                if (SetProperty(ref viewerPage, value))
                    EventAggregator.GetEvent<ViewerPageChangedEvent>().Publish();
            }
        }

        private MainViewerWindow view;

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
            this.view = view;
            view.Closed += WindowClosed;
            view.Activated += WindowActivated;
            view.Deactivated += WindowDeactivated;

            EventAggregator.GetEvent<ViewerPageChangeEvent>().Subscribe(ViewerPageChange);
            EventAggregator.GetEvent<MainViewerCloseEvent>().Subscribe(() => view.Close());

            videoPage = new VideoViewer() { WindowId = view.WindowId };
            imagePage = new ImageViewer(EventAggregator) { WindowId = view.WindowId };

            InitViewerChanged();

            dataManager.MainViewerOpend = true;
            EventAggregator.GetEvent<MainViewerOpendEvent>().Publish();
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(MainViewerWindow view)
        {
            EventAggregator.GetEvent<MainViewerUnloadEvent>().Publish(view.WindowId);

            EventAggregator.GetEvent<ViewerPageChangeEvent>().Unsubscribe(ViewerPageChange);
            EventAggregator.GetEvent<MainViewerCloseEvent>().Unsubscribe(() => this.view.Close());
        }

        /// <summary>
        /// 뷰어 변경
        /// </summary>
        private void InitViewerChanged()
        {
            string viewerName = dataManager.ViewerName;
            if (viewerName == nameof(VideoViewer))
            {
                Title = "Video Viewer - #" + view.WindowId;
                ViewerPage = videoPage;
            }
            else
            {
                Title = "Image Viewer - #" + view.WindowId;
                ViewerPage = imagePage;
            }
        }

        /// <summary>
        /// 뷰어 변경
        /// </summary>
        private void ViewerPageChange()
        {
            if (view.WindowId == dataManager.MainWindowId)
            {
                InitViewerChanged();
            }
        }

        /// <summary>
        /// 창이 활성화 상태일 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowActivated(object sender, EventArgs e)
        {
            if (view.IsActive)
            {
                dataManager.MainWindowId = view.WindowId;
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
