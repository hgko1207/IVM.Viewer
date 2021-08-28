using DevExpress.Xpf.Docking;
using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using IVM.Studio.Models.Events;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WPFDrawing = System.Windows.Media;

namespace IVM.Studio.Models
{
    public class I3DRecordInfo : BindableBase
    {
        I3DWcfServer wcfserver;
        IEventAggregator eventAggregator;

        DispatcherTimer timer;
        DateTime startTime;

        int sceneIdx = 1;

        //bool ffmpegInit = false;
        bool ignoreStopEvent = false;

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
                if (SetProperty(ref recordTarget2, value))
                {
                    RecordTarget1 = !recordTarget2;
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
            set
            {
                if (value)
                {
                    if (ScenePlay())
                        SetProperty(ref isPlaying, value);
                }
                else
                {
                    if (!ignoreStopEvent)
                        eventAggregator.GetEvent<I3DRecordPreviewPauseEvent>().Publish();
                    SetProperty(ref isPlaying, value);
                }
            }
        }

        private float currentSeconds = 0;
        public float CurrentSeconds
        {
            get => currentSeconds;
            set
            {
                if (SetProperty(ref currentSeconds, value))
                {
                    eventAggregator.GetEvent<I3DRecordPreviewSetPosEvent>().Publish(currentSeconds);
                }
            }
        }

        private float totalSeconds = 0;
        public float TotalSeconds
        {
            get => totalSeconds;
            set => SetProperty(ref totalSeconds, value);
        }

        private bool sceneGridEnable = true;
        public bool SceneGridEnable
        {
            get => sceneGridEnable;
            set => SetProperty(ref sceneGridEnable, value);
        }

        public enum CodecType
        {
            [Display(Name = "H264", Order = 0)]
            H264 = 0,
            [Display(Name = "MPEG4", Order = 1)]
            MPEG4 = 1,
        }

        private CodecType exportCodec = CodecType.H264;
        public CodecType ExportCodec
        {
            get => exportCodec;
            set => SetProperty(ref exportCodec, value);
        }

        public ICommand ChangedSelectedCommand { get; private set; }
        public ICommand SceneAddCommand { get; private set; }
        public ICommand SceneDeleteCommand { get; private set; }
        public ICommand SceneUpwardCommand { get; private set; }
        public ICommand SceneDownwardCommand { get; private set; }
        public ICommand ScenePauseCommand { get; private set; }
        public ICommand SceneStopCommand { get; private set; }
        public ICommand TrimLeftCommand { get; private set; }
        public ICommand TrimRightCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }

        private void ClearFolder(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            // remove all files
            foreach (FileInfo fi in dir.GetFiles())
                fi.Delete();

            // remove dir recursive
            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                di.Delete();
            }
        }

        public I3DRecordInfo(IContainerExtension container, IEventAggregator eventAggregator)
        {
            wcfserver = container.Resolve<I3DWcfServer>();
            this.eventAggregator = eventAggregator;

            ChangedSelectedCommand = new DelegateCommand(ChangedSelected);
            SceneAddCommand = new DelegateCommand(SceneAdd);
            SceneDeleteCommand = new DelegateCommand(SceneDelete);
            SceneUpwardCommand = new DelegateCommand(SceneUpward);
            SceneDownwardCommand = new DelegateCommand(SceneDownward);

            ScenePauseCommand = new DelegateCommand(ScenePause);
            SceneStopCommand = new DelegateCommand(SceneStop);

            TrimLeftCommand = new DelegateCommand(TrimLeft);
            TrimRightCommand = new DelegateCommand(TrimRight);
            ExportCommand = new DelegateCommand(Export);

            // if exist video working directory, clean up.
            if (Directory.Exists(sceneWorkingPath))
                ClearFolder(sceneWorkingPath);

            // remake working dir.
            Directory.CreateDirectory(sceneWorkingPath);

            // recording timer
            timer = new DispatcherTimer();
            timer.Tick += UpdateTick;

            //if (!ffmpegInit)
            //{
            //    FFmpegLoader.FFmpegPath = @".\ffmpeg";
            //    ffmpegInit = true;
            //}
        }

        private void Export()
        {
            VistaSaveFileDialog dialog = new VistaSaveFileDialog
            {
                DefaultExt = ".png",
                Filter = "MP4 image file(*.mp4)|*.mp4|AVI image file(*.avi)|*.avi",
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                LoadingWindow loading = new LoadingWindow();

                Task.Run(() =>
                {
                    string lstPath = Directory.GetCurrentDirectory() + sceneWorkingPath.Replace(".", "") + "lst.txt";
                    bool valid = false;

                    using (StreamWriter writer = File.CreateText(lstPath))
                    {
                        foreach (I3DSceneInfo info in SceneCollection)
                        {
                            string fn = Directory.GetCurrentDirectory() + sceneWorkingPath.Replace(".", "") + info.Id + ".mp4";
                            if (!File.Exists(fn))
                                continue;

                            writer.WriteLine("file '{0}'", fn);
                            valid = true;
                        }
                    }

                    if (!valid)
                    {
                        loading.loading = false;
                        return;
                    }

                    string codec = "";
                    
                    //string ext = Path.GetExtension(dialog.FileName);
                    if (ExportCodec == CodecType.H264)
                        codec = "h264";
                    else if (ExportCodec == CodecType.MPEG4)
                        codec = "mpeg4";
                    else
                    {
                        loading.loading = false;
                        return;
                    }

                    string arg = $@"-safe 0 -f concat -i ""{lstPath}"" -c copy -vcodec ""{codec}"" ""{dialog.FileName}""";
                    using (Process process = Process.Start(@".\ffmpeg\ffmpeg.exe", arg))
                    {
                        process.WaitForExit();
                    }

                    loading.loading = false;
                });

                loading.ShowDialog();
            }
        }

        private async void TrimLeft()
        {
            if (isPlaying)
                return;

            if (isRecording)
                return;

            if (SelectedSceneInfo == null)
                return;

            eventAggregator.GetEvent<I3DRecordPreviewCloseEvent>().Publish();

            int sceneId = SelectedSceneInfo.Id;

            string inPath = Directory.GetCurrentDirectory() + sceneWorkingPath + sceneId + ".mp4";
            string outPath = Directory.GetCurrentDirectory() + sceneWorkingPath + sceneId + ".wrk.mp4";

            TimeSpan from = TimeSpan.FromSeconds(CurrentSeconds);
            TimeSpan to = TimeSpan.FromSeconds(TotalSeconds);

            LoadingWindow loading = new LoadingWindow();

            Task.Run(() => {
                string arg = $@"-i ""{inPath}"" -ss {from:hh\:mm\:ss} -t {to - from:hh\:mm\:ss} -c copy ""{outPath}""";
                using (Process process = Process.Start(@".\ffmpeg\ffmpeg.exe", arg))
                {
                    process.WaitForExit();
                }

                while (true)
                {
                    try
                    {
                        File.Delete(inPath);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(1);
                    }
                }

                while (true)
                {
                    try
                    {
                        File.Move(outPath, inPath);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(1);
                    }
                }

                loading.loading = false;
            });

            loading.ShowDialog();

            await Task.Run(() =>
            {
                while (true)
                {
                    if (!loading.loading)
                        break;
                }
            });

            eventAggregator.GetEvent<I3DRecordPreviewOpenEvent>().Publish(inPath);
        }

        private async void TrimRight()
        {
            if (isPlaying)
                return;

            if (isRecording)
                return;

            if (SelectedSceneInfo == null)
                return;

            eventAggregator.GetEvent<I3DRecordPreviewCloseEvent>().Publish();

            int sceneId = SelectedSceneInfo.Id;

            string inPath = Directory.GetCurrentDirectory() + sceneWorkingPath + sceneId + ".mp4";
            string outPath = Directory.GetCurrentDirectory() + sceneWorkingPath + sceneId + ".wrk.mp4";

            TimeSpan from = TimeSpan.FromSeconds(0);
            TimeSpan to = TimeSpan.FromSeconds(CurrentSeconds);

            LoadingWindow loading = new LoadingWindow();

            Task.Run(() => {
                string arg = $@"-i ""{inPath}"" -ss {from:hh\:mm\:ss} -t {to - from:hh\:mm\:ss} -c copy ""{outPath}""";
                using (Process process = Process.Start(@".\ffmpeg\ffmpeg.exe", arg))
                {
                    process.WaitForExit();
                }

                while (true)
                {
                    try
                    {
                        File.Delete(inPath);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }

                while (true)
                {
                    try
                    {
                        File.Move(outPath, inPath);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }

                loading.loading = false;
            });

            loading.ShowDialog();

            await Task.Run(() =>
            {
                while (true)
                {
                    if (!loading.loading)
                        break;
                }
            });

            eventAggregator.GetEvent<I3DRecordPreviewOpenEvent>().Publish(inPath);
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            if (IsRecording)
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

            TopmostPreview();

            eventAggregator.GetEvent<I3DRecordPreviewPlayEvent>().Publish(scenePath);

            SceneGridEnable = false;

            return true;
        }

        public void TopmostPreview()
        {
            MainWindow mainWindow = (IVM.Studio.Views.MainWindow)System.Windows.Application.Current.MainWindow;

            DocumentPanel docp = (DocumentPanel)mainWindow.i3drvdocp;
            if (docp.Parent is DocumentGroup)
            {
                DocumentGroup docg = (DocumentGroup)docp.Parent;
                docg.SelectedTabIndex = docg.Items.IndexOf(docp);
            }
        }

        public void ScenePause()
        {
            if (IsRecording)
            {
                RecordStopInternal();
                IsRecording = false;
            }
            else if (IsPlaying)
            {
                eventAggregator.GetEvent<I3DRecordPreviewPauseEvent>().Publish();
                ignoreStopEvent = true;
                IsPlaying = false;
                ignoreStopEvent = false;
                SceneGridEnable = true;
            }
        }

        public void SceneStop()
        {
            if (IsRecording)
            {
                RecordStopInternal();
                IsRecording = false;
            }
            else
            {
                eventAggregator.GetEvent<I3DRecordPreviewStopEvent>().Publish();
                ignoreStopEvent = true;
                IsPlaying = false;
                ignoreStopEvent = false;
                SceneGridEnable = true;
            }
        }

        private void RecordStopInternal()
        {
            wcfserver.channel1.StopRecordVideo();
            wcfserver.channel2.StopRecordVideo();
            timer.Stop();
            SceneGridEnable = true;

            SelectedSceneInfo.Duration = CurrentSeconds;
            SceneCollection[SceneCollection.IndexOf(SelectedSceneInfo)] = SelectedSceneInfo;

            if (SelectedSceneInfo == null)
                return;

            int sceneId = SelectedSceneInfo.Id;

            string scenePath = Directory.GetCurrentDirectory() + sceneWorkingPath + sceneId + ".mp4";

            eventAggregator.GetEvent<I3DRecordPreviewOpenEvent>().Publish(scenePath);
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

            eventAggregator.GetEvent<I3DRecordPreviewCloseEvent>().Publish();

            int sceneId = SelectedSceneInfo.Id;

            string scenePath = Directory.GetCurrentDirectory() + sceneWorkingPath + sceneId + ".mp4";

            if (File.Exists(scenePath))
            {
                while (true)
                {
                    try
                    {
                        File.Delete(scenePath);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(1);
                    }
                }
            }

            if (recordTarget1)
                wcfserver.channel1.StartRecordVideo(scenePath);
            else
                wcfserver.channel2.StartRecordVideo(scenePath);

            timer.Start();
            startTime = DateTime.Now;
            CurrentSeconds = 0;
            TotalSeconds = 0;
            SceneGridEnable = false;

            return true;
        }

        private void SceneUpward()
        {
            if (SelectedSceneInfo == null)
                return;

            int idx = SceneCollection.IndexOf(SelectedSceneInfo);
            if (idx <= 0)
                return;

            SceneCollection.Move(idx, idx - 1);
        }

        private void SceneDownward()
        {
            if (SelectedSceneInfo == null)
                return;

            int idx = SceneCollection.IndexOf(SelectedSceneInfo);
            if (idx >= SceneCollection.Count - 1)
                return;

            SceneCollection.Move(idx, idx + 1);
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
