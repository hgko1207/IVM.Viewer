using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Views.UserControls;
using Prism.Events;
using Prism.Ioc;
using System;
using System.IO;
using Vlc.DotNet.Core.Interops.Signatures;

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

        private DisplayParam displayParam;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public VideoViewerViewModel(IContainerExtension container) : base(container)
        {
            EventAggregator.GetEvent<DisplayVideoEvent>().Subscribe(InitialPlayVideo);
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(VideoViewer view)
        {
            this.view = view;

            view.MediaPlayer.SourceProvider.CreatePlayer(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "libvlc")));

            if (displayParam != null)
                view.MediaPlayer.SourceProvider.MediaPlayer.Play(displayParam.FileInfo);

            EventAggregator.GetEvent<PlayVideoEvent>().Subscribe(PlayVideo, ThreadOption.BackgroundThread);
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(VideoViewer view)
        {
            EventAggregator.GetEvent<PlayVideoEvent>().Unsubscribe(PlayVideo);
        }

        /// <summary>
        /// 비디오 표출
        /// </summary>
        /// <param name="param"></param>
        private void InitialPlayVideo(DisplayParam param)
        {
            this.displayParam = param;
        }

        /// <summary>
        /// Play Video
        /// </summary>
        private void PlayVideo()
        {
            if (view.MediaPlayer.SourceProvider.MediaPlayer.State == MediaStates.Ended)
                view.MediaPlayer.SourceProvider.MediaPlayer.Stop();

            view.MediaPlayer.SourceProvider.MediaPlayer.Play();
        }
    }
}
