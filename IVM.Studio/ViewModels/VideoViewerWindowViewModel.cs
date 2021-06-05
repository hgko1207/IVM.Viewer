using IVM.Studio.Mvvm;
using IVM.Studio.MvvM;
using IVM.Studio.Views;
using Prism.Ioc;
using System;

/**
 * @Class Name : VideoViewerWindowViewModel.cs
 * @Description : 비디오 뷰어 뷰모델
 * @Modification Information
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.05     고형균              최초생성
 *
 * @author 고형균
 * @since 2020.06.05
 * @version 1.0
 */
namespace IVM.Studio.ViewModels
{
    public class VideoViewerWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<VideoViewerWindow>
    {
        private VideoViewerWindow view;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public VideoViewerWindowViewModel(IContainerExtension container) : base(container)
        {
            Title = "Video Viewer";
        }

        public void OnLoaded(VideoViewerWindow view)
        {
            this.view = view;
        }

        public void OnUnloaded(VideoViewerWindow view)
        {
        }
    }
}
