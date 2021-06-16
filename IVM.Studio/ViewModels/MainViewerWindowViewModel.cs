using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Prism.Ioc;
using System;
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
                {

                }
            }
        }

        private MainViewerWindow view;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public MainViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Image Viewer";

            EventAggregator.GetEvent<ViewerPageChangedEvent>().Subscribe(ViewerChanged);
            EventAggregator.GetEvent<MainViewerCloseEvent>().Subscribe(() => view.Close());
            
            ViewerChanged();
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(MainViewerWindow view)
        {
            this.view = view;
            view.Closed += WindowClosed;
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(MainViewerWindow view)
        {
            EventAggregator.GetEvent<MainViewerCloseEvent>().Unsubscribe(() => view.Close());
        }

        /// <summary>
        /// 뷰어 변경
        /// </summary>
        private void ViewerChanged()
        {
            if (ViewerPage != Container.Resolve<DataManager>().ViewerPage)
                ViewerPage = Container.Resolve<DataManager>().ViewerPage;
        }

        /// <summary>
        /// Window 종료 시킬 때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosed(object sender, EventArgs e)
        {
            EventAggregator.GetEvent<MainViewerClosedEvent>().Publish();
        }
    }
}
