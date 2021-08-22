using DevExpress.Xpf.Docking;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DFreeRecordingPanelViewModel : ViewModelBase
    {
        I3DWcfServer wcfserver;

        DispatcherTimer timer;
        DateTime startTime;

        int sceneIdx = 1;

        const string sceneWorkingPath = @".\video\";

        private ObservableCollection<I3DSceneInfo> sceneCollection = new ObservableCollection<I3DSceneInfo>();
        public ObservableCollection<I3DSceneInfo> SceneCollection
        {
            get => sceneCollection;
            set => SetProperty(ref sceneCollection, value);
        }

        private I3DSceneInfo selectedSceneInfo;
        public I3DSceneInfo SelectedSceneInfo
        {
            get => selectedSceneInfo;
            set => SetProperty(ref selectedSceneInfo, value);
        }

        private bool recordTarget1 = true;
        public bool RecordTarget1
        {
            get => recordTarget1;
            set
            {
                if (SetProperty(ref recordTarget1, value))
                {
                    RecordTarget2 = !recordTarget1;
                }
            }
        }

        private bool recordTarget2 = false;
        public bool RecordTarget2
        {
            get => recordTarget2;
            set
            {
                if (value)
                {
                    if (ScenePlay())
                        SetProperty(ref isRecording, value);
                }
                else
                {
                    PlayStopInternal();
                    SetProperty(ref isRecording, value);
                }
            }
        }

        private bool isRecording = false;
        public bool IsRecording
        {
            get => isRecording;
            set
            {
                if (value)
                {
                    if (SceneRecord())
                        SetProperty(ref isRecording, value);
                }
                else
                {
                    RecordStopInternal();
                    SetProperty(ref isRecording, value);
                }
            }
        }

        private bool isPlaying = false;
        public bool IsPlaying
        {
            get => isPlaying;
            set => SetProperty(ref isPlaying, value);
        }

        private float currentSeconds = 0;
        public float CurrentSeconds
        {
            get => currentSeconds;
            set => SetProperty(ref currentSeconds, value);
        }

        public ICommand ChangedSelectedCommand { get; private set; }
        public ICommand SceneAddCommand { get; private set; }
        public ICommand SceneDeleteCommand { get; private set; }
        public ICommand SceneUpwardCommand { get; private set; }
        public ICommand SceneDownwardCommand { get; private set; }
        public ICommand ScenePauseCommand { get; private set; }
        public ICommand SceneStopCommand { get; private set; }

        private void clearFolder(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            // remove all files
            foreach (FileInfo fi in dir.GetFiles())
                fi.Delete();

            // remove dir recursive
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DFreeRecordingPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();

            ChangedSelectedCommand = new DelegateCommand(ChangedSelected);
            SceneAddCommand = new DelegateCommand(SceneAdd);
            SceneDeleteCommand = new DelegateCommand(SceneDelete);
            SceneUpwardCommand = new DelegateCommand(SceneUpward);
            SceneDownwardCommand = new DelegateCommand(SceneDownward);

            ScenePauseCommand = new DelegateCommand(ScenePause);
            SceneStopCommand = new DelegateCommand(SceneStop);

            // if exist video working directory, clean up.
            if (Directory.Exists(sceneWorkingPath))
                clearFolder(sceneWorkingPath);

            // remake working dir.
            Directory.CreateDirectory(sceneWorkingPath);

            // recording timer
            timer = new DispatcherTimer();
            timer.Tick += UpdateTick;
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            CurrentSeconds = (float)(DateTime.Now - startTime).TotalSeconds;
        }

        private bool ScenePlay()
        {
            if (isRecording)
                return false;

            if (SelectedSceneInfo == null)
                return false;

            int sceneId = SelectedSceneInfo.Id;

            string scenePath = Directory.GetCurrentDirectory() + sceneWorkingPath + sceneId + ".mp4";

            if (!File.Exists(scenePath))
                return false;

            MainWindow mainWindow = (IVM.Studio.Views.MainWindow)System.Windows.Application.Current.MainWindow;

            DocumentPanel docp = (DocumentPanel)mainWindow.i3drvdocp;
            DocumentGroup docg = (DocumentGroup)docp.Parent;
            docg.SelectedTabIndex = docg.Items.IndexOf(docp);

            return true;
        }

        private void ScenePause()
        {
            if (IsRecording)
            {
                RecordStopInternal();
                IsRecording = false;
            }

            if (IsPlaying)
            {
                PlayStopInternal();
                IsPlaying = false;
            }
        }

        private void SceneStop()
        {
            if (IsRecording)
            {
                RecordStopInternal();
                IsRecording = false;
            }

            if (IsPlaying)
            {
                PlayStopInternal();
                IsPlaying = false;
            }
        }

        private void PlayStopInternal()
        {

        }

        private void RecordStopInternal()
        {
            wcfserver.channel1.StopRecordVideo();
            wcfserver.channel2.StopRecordVideo();
            timer.Stop();

            SelectedSceneInfo.Duration = CurrentSeconds;
            I3DSceneInfo s = SelectedSceneInfo.Clone();

            int idx = SceneCollection.IndexOf(SelectedSceneInfo);
            SceneCollection.Remove(SelectedSceneInfo);
            SceneCollection.Insert(idx, s);
        }

        private bool SceneRecord()
        {
            if (isPlaying)
                return false;

            if (SceneCollection.Count <= 0)
            {
                SceneAdd();
            }
            else if (SelectedSceneInfo == null)
                return false;

            int sceneId = SelectedSceneInfo.Id;

            string scenePath = Directory.GetCurrentDirectory() + sceneWorkingPath + sceneId + ".mp4";

            if (File.Exists(scenePath))
                File.Delete(scenePath);

            if (recordTarget1)
                wcfserver.channel1.StartRecordVideo(scenePath);
            else
                wcfserver.channel2.StartRecordVideo(scenePath);

            timer.Start();
            startTime = DateTime.Now;
            CurrentSeconds = 0;

            return true;
        }

        private void SceneUpward()
        {
            if (SelectedSceneInfo == null)
                return;

            int idx = SceneCollection.IndexOf(SelectedSceneInfo);
            if (idx <= 0)
                return;

            idx--;

            I3DSceneInfo s = SelectedSceneInfo.Clone();

            SceneCollection.Remove(SelectedSceneInfo);
            SceneCollection.Insert(idx, s);

            SelectedSceneInfo = s;
        }

        private void SceneDownward()
        {
            if (SelectedSceneInfo == null)
                return;

            int idx = SceneCollection.IndexOf(SelectedSceneInfo);
            if (idx >= SceneCollection.Count - 1)
                return;

            idx++;

            I3DSceneInfo s = SelectedSceneInfo.Clone();

            SceneCollection.Remove(SelectedSceneInfo);
            SceneCollection.Insert(idx, s);

            SelectedSceneInfo = s;
        }

        private void SceneDelete()
        {
            if (SelectedSceneInfo == null)
                return;

            SceneCollection.Remove(SelectedSceneInfo);
        }

        private void SceneAdd()
        {
            I3DSceneInfo s = new I3DSceneInfo();
            s.Name = "Free-hand Scene #" + sceneIdx;
            s.Id = sceneIdx;

            SceneCollection.Add(s);

            SelectedSceneInfo = s;

            sceneIdx++;
        }

        private void ChangedSelected()
        {
        }
    }
}
