using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.IO;
using System.Windows.Input;
using Vlc.DotNet.Core;
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
        private FileInfo currentFile;
        public FileInfo CurrentFile
        {
            get => currentFile;
            set
            {
                if (SetProperty(ref currentFile, value))
                    RaisePropertyChanged(nameof(FileInfoText));
            }
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }

        private int videoLength;
        public int VideoLength
        {
            get => videoLength;
            set
            {
                if (SetProperty(ref videoLength, value))
                    RaisePropertyChanged(nameof(FileInfoText));
            }
        }
        private int videoCurrentTime;
        public int VideoCurrentTime
        {
            get => videoCurrentTime;
            set
            {
                if (SetProperty(ref videoCurrentTime, value))
                {
                    RaisePropertyChanged(nameof(FileInfoText));
                    IsPlaying = true;
                    EventAggregator.GetEvent<SeekVideoEvent>().Publish(TimeSpan.FromSeconds(value));
                }
            }
        }

        public string FileInfoText
        {
            get
            {
                if (CurrentFile == null)
                    return "File Name: (None)";
                else
                {
                    if (VideoLength > 0 && VideoCurrentTime <= VideoLength && VideoCurrentTime >= 0)
                        return $"File Name: {CurrentFile.Name}, Time: {TimeSpan.FromSeconds(VideoCurrentTime):mm\\:ss} / {TimeSpan.FromSeconds(VideoLength):mm\\:ss}";
                    else
                        return $"File Name: {CurrentFile.Name}";
                }
            }
        }

        public ICommand TrimCommand { get; private set; }
        public ICommand PlayPauseCommand { get; private set; }
        public ICommand StopCommand { get; private set; }

        private VideoViewer view;

        private DisplayParam displayParam;

        private (TimeSpan Current, TimeSpan Total) cachedPreviousTime;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public VideoViewerViewModel(IContainerExtension container) : base(container)
        {
            TrimCommand = new DelegateCommand(Trim);
            PlayPauseCommand = new DelegateCommand(PlayPauseVideo);
            StopCommand = new DelegateCommand(StopVideo);

            EventAggregator.GetEvent<DisplayVideoEvent>().Subscribe(InitialPlayVideo, ThreadOption.UIThread);
            EventAggregator.GetEvent<DisplayImageEvent>().Subscribe(StopVideo, ThreadOption.BackgroundThread);
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(VideoViewer view)
        {
            this.view = view;

            view.MediaPlayer.SourceProvider.CreatePlayer(new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "libvlc")));
            view.MediaPlayer.SourceProvider.MediaPlayer.TimeChanged += MediaPlayerTimeChanged;
            view.MediaPlayer.SourceProvider.MediaPlayer.EndReached += MediaPlayerEndReached; ;

            if (displayParam != null)
                view.MediaPlayer.SourceProvider.MediaPlayer.Play(displayParam.FileInfo);

            EventAggregator.GetEvent<PlayVideoEvent>().Subscribe(PlayVideo, ThreadOption.BackgroundThread);
            EventAggregator.GetEvent<PauseVideoEvent>().Subscribe(PauseVideo, ThreadOption.BackgroundThread);
            EventAggregator.GetEvent<SeekVideoEvent>().Subscribe(SeekVideo, ThreadOption.BackgroundThread);

            EventAggregator.GetEvent<PlayingVideoEvent>().Subscribe(PlayingVideo);
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(VideoViewer view)
        {
            EventAggregator.GetEvent<PlayVideoEvent>().Unsubscribe(PlayVideo);
            EventAggregator.GetEvent<PlayVideoEvent>().Unsubscribe(PauseVideo);
            EventAggregator.GetEvent<SeekVideoEvent>().Unsubscribe(SeekVideo);

            EventAggregator.GetEvent<PlayingVideoEvent>().Unsubscribe(PlayingVideo);
        }

        /// <summary>
        /// MediaPlayerTimeChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayerTimeChanged(object sender, VlcMediaPlayerTimeChangedEventArgs e)
        {
            (TimeSpan Current, TimeSpan Total) time;
            time.Current = TimeSpan.FromMilliseconds(e.NewTime);
            time.Total = TimeSpan.FromMilliseconds(view.MediaPlayer.SourceProvider.MediaPlayer.Length);
            // 딜레이 0.5초
            if ((cachedPreviousTime.Current - time.Current).Duration() > TimeSpan.FromMilliseconds(500) || cachedPreviousTime.Total != time.Total)
            {
                cachedPreviousTime = time;
                EventAggregator.GetEvent<PlayingVideoEvent>().Publish(new PlayingVideoParam(time.Current, time.Total));
            }
        }

        /// <summary>
        /// MediaPlayerEndReached
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MediaPlayerEndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            FinishedVideo();
        }

        /// <summary>
        /// 비디오 표출
        /// </summary>
        /// <param name="param"></param>
        private void InitialPlayVideo(DisplayParam param)
        {
            this.displayParam = param;

            IsPlaying = true;
            CurrentFile = param.FileInfo;
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

        /// <summary>
        /// Pause Video
        /// </summary>
        private void PauseVideo()
        {
            view.MediaPlayer.SourceProvider.MediaPlayer.Pause();
        }

        /// <summary>
        /// Seek Video
        /// </summary>
        /// <param name="timeToSeek"></param>
        private void SeekVideo(TimeSpan timeToSeek)
        {
            switch (view.MediaPlayer.SourceProvider.MediaPlayer.State)
            {
                case MediaStates.Ended:
                    view.MediaPlayer.SourceProvider.MediaPlayer.Stop();
                    view.MediaPlayer.SourceProvider.MediaPlayer.Play();
                    break;
                case MediaStates.Stopped:
                    view.MediaPlayer.SourceProvider.MediaPlayer.Play();
                    break;
            }

            long time = (long)timeToSeek.TotalMilliseconds;
            view.MediaPlayer.SourceProvider.MediaPlayer.Time = time;
        }

        /// <summary>
        /// Playing Video
        /// </summary>
        /// <param name="param"></param>
        private void PlayingVideo(PlayingVideoParam param)
        {
            VideoLength = (int)param.VideoLength.TotalSeconds;
            UpdateVideoCurrentTimeWithoutSeek((int)param.VideoCurrentTime.TotalSeconds);
        }

        /// <summary>
        /// Finished Video
        /// </summary>
        private void FinishedVideo()
        {
            IsPlaying = false;

            UpdateVideoCurrentTimeWithoutSeek(VideoLength);
            Container.Resolve<SlideShowService>().ContinueSlideShow();
        }

        /// <summary>
        /// Trim
        /// </summary>
        private void Trim()
        {

        }

        /// <summary>
        /// 실행 중지 이벤트
        /// </summary>
        private void PlayPauseVideo()
        {
            if (IsPlaying)
            {
                IsPlaying = false;
                EventAggregator.GetEvent<PauseVideoEvent>().Publish();
            }
            else
            {
                IsPlaying = true;
                EventAggregator.GetEvent<PlayVideoEvent>().Publish();
            }
        }

        /// <summary>
        /// Stop Video
        /// </summary>
        private void StopVideo()
        {
            IsPlaying = false;
            UpdateVideoCurrentTimeWithoutSeek(0);

            EventAggregator.GetEvent<StopVideoEvent>().Publish();
            Container.Resolve<SlideShowService>().ContinueSlideShow();
        }

        /// <summary>
        /// UpdateVideoCurrentTimeWithoutSeek
        /// </summary>
        /// <param name="newValue"></param>
        private void UpdateVideoCurrentTimeWithoutSeek(int newValue)
        {
            if (SetProperty(ref videoCurrentTime, newValue, nameof(VideoCurrentTime)))
                RaisePropertyChanged(nameof(FileInfoText));
        }

        /// <summary>
        /// Stop Video
        /// </summary>
        /// <param name="param"></param>
        private void StopVideo(DisplayParam param)
        {
            StopVideo();
        }
    }
}
