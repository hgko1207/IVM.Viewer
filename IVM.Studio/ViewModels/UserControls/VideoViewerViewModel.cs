using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Views.UserControls;
using Prism.Ioc;
using System;

/**
 * @Class Name : VideoViewerViewModel.cs
 * @Description : 비디오 뷰어 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.16     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.06.16
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class VideoViewerViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<VideoViewer>
    {
        private VideoViewer view;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public VideoViewerViewModel(IContainerExtension container) : base(container)
        {
            
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(VideoViewer view)
        {
            this.view = view;
            EventAggregator.GetEvent<DisplayVideoEvent>().Subscribe(InitialPlayVideo);
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(VideoViewer view)
        {
            EventAggregator.GetEvent<DisplayVideoEvent>().Unsubscribe(InitialPlayVideo);
        }

        /// <summary>
        /// 비디오 표출
        /// </summary>
        /// <param name="param"></param>
        private void InitialPlayVideo(DisplayParam param)
        {
            view.MediaPlayer.SourceProvider.MediaPlayer.Play(param.FileInfo);
        }
    }
}
