using IVM.Studio.Mvvm;
using IVM.Studio.Views;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

/**
 * @Class Name : VideoTrimWindowViewModel.cs
 * @Description : 영상 Trim 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.06.22     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.06.22
 * @version 1.0
 */
namespace IVM.Studio.ViewModels
{
    public class VideoTrimWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<VideoTrimWindow>
    {
        private int fromMin;
        public int FromMin
        {
            get => fromMin;
            set => SetProperty(ref fromMin, value);
        }

        private int fromSec;
        public int FromSec
        {
            get => fromSec;
            set => SetProperty(ref fromSec, value);
        }

        private int toMin;
        public int ToMin
        {
            get => toMin;
            set => SetProperty(ref toMin, value);
        }

        private int toSec;
        public int ToSec
        {
            get => toSec;
            set => SetProperty(ref toSec, value);
        }

        public FileInfo File;

        public TimeSpan Length;

        public ICommand TrimCommand { get; private set; }
        public ICommand ClosedCommand { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public VideoTrimWindowViewModel(IContainerExtension container) : base(container)
        {
            TrimCommand = new DelegateCommand(Trim);
            ClosedCommand = new DelegateCommand(Closed);
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(VideoTrimWindow view)
        {
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(VideoTrimWindow view)
        {
        }

        /// <summary>
        /// Trim
        /// </summary>
        private void Trim()
        {
            TimeSpan from = new TimeSpan(0, FromMin, FromSec);
            TimeSpan to = new TimeSpan(0, ToMin, ToSec);
            if (to > Length || from >= Length || from >= to || from < TimeSpan.Zero || to <= TimeSpan.Zero)
            {
                return;
            }

            VistaSaveFileDialog dialog = new VistaSaveFileDialog
            {
                DefaultExt = ".avi",
                Filter = "AVI 동영상 파일|.avi"
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Task.Run(() => {
                    using (Process process = Process.Start(@".\ffmpeg\ffmpeg.exe", $@"-i ""{File.FullName}"" -ss {from:hh\:mm\:ss} -t {to - from:hh\:mm\:ss} -c copy ""{dialog.FileName}"""))
                    {
                        process.WaitForExit();
                    }
                });
            }
        }

        /// <summary>
        /// 닫기 이벤트
        /// </summary>
        private void Closed()
        {

        }
    }
}
