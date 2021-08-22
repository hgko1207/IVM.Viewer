using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DRecordPreviewerViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<I3DRecordPreviewer>
    {
        I3DRecordPreviewer view;

        DataManager datamanager;
        public I3DRecordInfo I3DRecordInfo { get; set; }

        DispatcherTimer timer;

        string openPath = "";
        bool loading = false;
        bool playing = false;

        public ICommand MediaOpenedCommand { get; private set; }
        public ICommand MediaEndedCommand { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DRecordPreviewerViewModel(IContainerExtension container) : base(container)
        {
            datamanager = container.Resolve<DataManager>();

            I3DRecordInfo = datamanager.I3DRecordInfo;

            MediaOpenedCommand = new DelegateCommand(MediaOpened);
            MediaEndedCommand = new DelegateCommand(MediaEnded);

            EventAggregator.GetEvent<I3DRecordPreviewOpenEvent>().Subscribe(Open);
            EventAggregator.GetEvent<I3DRecordPreviewPlayEvent>().Subscribe(Play);
            EventAggregator.GetEvent<I3DRecordPreviewStopEvent>().Subscribe(Stop);
            EventAggregator.GetEvent<I3DRecordPreviewPauseEvent>().Subscribe(Pause);
            EventAggregator.GetEvent<I3DRecordPreviewSetPosEvent>().Subscribe(SetPos);

            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1) };
            timer.Tick += UpdateTick;
        }

        public void OnLoaded(I3DRecordPreviewer view)
        {
            this.view = view;
        }

        public void OnUnloaded(I3DRecordPreviewer view)
        {
        }

        private void MediaOpened()
        {
            float totalSeconds = (float)view.media.NaturalDuration.TimeSpan.TotalSeconds;

            I3DRecordInfo.TotalSeconds = totalSeconds;
            I3DRecordInfo.SelectedSceneInfo.Duration = totalSeconds;
            I3DRecordInfo.SceneCollection[I3DRecordInfo.SceneCollection.IndexOf(I3DRecordInfo.SelectedSceneInfo)] = I3DRecordInfo.SelectedSceneInfo;

            loading = false;
        }
        private void MediaEnded()
        {
            //if (!playing)
            //    return;

            //I3DRecordInfo.IsPlaying = false;
        }
                
        private void UpdateTick(object sender, EventArgs e)
        {
            I3DRecordInfo.CurrentSeconds = (float) view.media.Position.TotalSeconds;

            if (loading)
                I3DRecordInfo.TotalSeconds = I3DRecordInfo.CurrentSeconds;
            else if (playing && I3DRecordInfo.TotalSeconds < I3DRecordInfo.CurrentSeconds)
                I3DRecordInfo.IsPlaying = false;
        }

        private void Open(string path)
        {
            if (openPath == path)
                return;

            view.media.Source = new Uri(path);
            view.media.Volume = 0.5;
            view.media.SpeedRatio = 1;
            view.media.ScrubbingEnabled = true;

            view.media.Play();
            view.media.Pause();

            openPath = path;
            loading = true;

            timer.Start(); // 선택한 파일을 실행
        }

        private void Play()
        {
            view.media.Play();
            playing = true;

            timer.Start(); // 선택한 파일을 실행
        }

        private void Stop()
        {
            view.media.Stop();
            view.media.Position = TimeSpan.FromSeconds(0);
            I3DRecordInfo.CurrentSeconds = 0;
            playing = false;

            timer.Stop();
        }

        private void Pause()
        {
            view.media.Pause();
            playing = false;

            timer.Stop();
        }

        private void SetPos(float s)
        {
            if (playing)
                return;

            //Console.WriteLine("{0}", s);

            view.media.Position = TimeSpan.FromSeconds(s);
        }
    }
}
