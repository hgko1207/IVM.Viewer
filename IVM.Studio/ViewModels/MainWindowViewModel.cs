using DevExpress.Xpf.WindowsUI;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using IVM.Studio.Views.UserControls;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/**
 * @Class Name : MainWindowViewModel.cs
 * @Description : 메인 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.03.29     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.03.29
 * @version 1.0
 */
namespace IVM.Studio.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<MainWindow>
    {
        private ObservableCollection<SlideInfo> slideInfoCollection;
        public ObservableCollection<SlideInfo> SlideInfoCollection => slideInfoCollection ?? (slideInfoCollection = new ObservableCollection<SlideInfo>());

        private SlideInfo selectedSlideInfo;
        public SlideInfo SelectedSlideInfo
        {
            get => selectedSlideInfo;
            set
            {
                if (SetProperty(ref selectedSlideInfo, value) && value != null)
                {
                    StopSlideshow();
                    EnableImageSliders(currentSlidesPath, value.Name);
                    DisplaySlide(true);
                }
            }
        }

        private ObservableCollection<MetadataModel> metadataCollection = new ObservableCollection<MetadataModel>();
        public ObservableCollection<MetadataModel> MetadataCollection
        {
            get => metadataCollection;
            set => SetProperty(ref metadataCollection, value);
        }

        private string selectedFilename;
        public string SelectedFilename
        {
            get => selectedFilename;
            set => SetProperty(ref selectedFilename, value);
        }

        public bool ZSSliderEnabled => ZSSliderMaximum > 1 && Container.Resolve<DataManager>().MainViewerOpend;

        private int _ZSSliderMinimum;
        public int ZSSliderMinimum
        {
            get => _ZSSliderMinimum;
            set => SetProperty(ref _ZSSliderMinimum, value);
        }

        private int _ZSSliderMaximum;
        public int ZSSliderMaximum
        {
            get => _ZSSliderMaximum;
            set
            {
                if (SetProperty(ref _ZSSliderMaximum, value))
                {
                    RaisePropertyChanged(nameof(ZSSliderText));
                    RaisePropertyChanged(nameof(ZSSliderEnabled));
                }
            }
        }

        private int _ZSSliderValue;
        public int ZSSliderValue
        {
            get => _ZSSliderValue;
            set
            {
                if (SetProperty(ref _ZSSliderValue, value))
                {
                    RaisePropertyChanged(nameof(ZSSliderText));
                    if (!disableSlidersEvent)
                        DisplaySlide(false);
                }
            }
        }

        public string ZSSliderText => $"{ZSSliderValue}/{ZSSliderMaximum}";

        private int currentPlayingSlider;
        public int CurrentPlayingSlider
        {
            get => currentPlayingSlider;
            set
            {
                SetProperty(ref currentPlayingSlider, value);
                RaisePropertyChanged(nameof(ZSSliderPlaying));
                RaisePropertyChanged(nameof(MSSliderPlaying));
                RaisePropertyChanged(nameof(MPSliderPlaying));
                RaisePropertyChanged(nameof(TLSliderPlaying));
            }
        }

        public bool ZSSliderPlaying => CurrentPlayingSlider == 0;
        public bool MSSliderPlaying => CurrentPlayingSlider == 1;
        public bool MPSliderPlaying => CurrentPlayingSlider == 2;
        public bool TLSliderPlaying => CurrentPlayingSlider == 3;

        private double slideShowFps;
        public double SlideShowFps
        {
            get => slideShowFps;
            set => SetProperty(ref slideShowFps, value);
        }

        private int slideShowRepeat;
        public int SlideShowRepeat
        {
            get => slideShowRepeat;
            set => SetProperty(ref slideShowRepeat, value);
        }

        public ICommand OpenFolderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand PreviousSlideCommand { get; private set; }
        public ICommand NextSlideCommand { get; private set; }

        public ICommand RotationCommand { get; private set; }
        public ICommand ReflectCommand { get; private set; }
        public ICommand RotationResetCommand { get; private set; }

        public ICommand PlaySlideShowCommand { get; private set; }

        private MainWindow view;

        private readonly UserControl imagePage;
        private readonly UserControl videoPage;
        private UserControl viewerPage;

        private readonly IEnumerable<string> imageFileExtensions;
        private readonly IEnumerable<string> videoFileExtensions;
        private IEnumerable<string> approvedExtensions => Enumerable.Concat(imageFileExtensions, videoFileExtensions);

        private string currentSlidesPath;
        private FileInfo currentFile;

        private bool disableSlidersEvent;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public MainWindowViewModel(IContainerExtension container) : base(container)
        {
            RegionManager.RegisterViewWithRegion("ImageAdjustmentPanel", typeof(ImageAdjustment));
            RegionManager.RegisterViewWithRegion("AnnotationPanel", typeof(Annotation));
            RegionManager.RegisterViewWithRegion("ColormapPanel", typeof(Colormap));
            RegionManager.RegisterViewWithRegion("DisplayControlPanel", typeof(DisplayControl));

            container.Resolve<DataManager>().Init(container, EventAggregator);

            OpenFolderCommand = new DelegateCommand(OpenFolder);
            RefreshCommand = new DelegateCommand(Refresh);
            PreviousSlideCommand = new DelegateCommand(PreviousSlide);
            NextSlideCommand = new DelegateCommand(NextSlide);

            RotationCommand = new DelegateCommand<string>(Rotation);
            ReflectCommand = new DelegateCommand<string>(Reflect);
            RotationResetCommand = new DelegateCommand(RotationReset);

            PlaySlideShowCommand = new DelegateCommand<string>(PlaySlideShow);

            EventAggregator.GetEvent<PlaySlideShowEvent>().Subscribe(InternalPlaySlideshow);
            EventAggregator.GetEvent<MainViewerOpendEvent>().Subscribe(SliderStateChanged);
            EventAggregator.GetEvent<MainViewerClosedEvent>().Subscribe(SliderStateChanged);

            imageFileExtensions = new[] { ".ivm" };
            videoFileExtensions = new[] { ".avi" };

            imagePage = new ImageViewer();
            videoPage = new VideoViewer();
            viewerPage = imagePage;

            CurrentPlayingSlider = -1;
            SlideShowFps = 5;
            SlideShowRepeat = 2;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(MainWindow view)
        {
            this.view = view;
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(MainWindow view)
        {
            EventAggregator.GetEvent<PlaySlideShowEvent>().Unsubscribe(InternalPlaySlideshow);
            EventAggregator.GetEvent<MainViewerOpendEvent>().Unsubscribe(SliderStateChanged);
            EventAggregator.GetEvent<MainViewerClosedEvent>().Unsubscribe(SliderStateChanged);
        }

        /// <summary>
        /// 폴더 열기
        /// </summary>
        private void OpenFolder()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (!string.IsNullOrEmpty(currentSlidesPath))
                folderBrowserDialog.SelectedPath = currentSlidesPath;

            if (folderBrowserDialog.ShowDialog().GetValueOrDefault())
                currentSlidesPath = folderBrowserDialog.SelectedPath;

            Refresh();
        }

        /// <summary>
        /// 새로고침 이벤트
        /// </summary>
        private void Refresh()
        {
            SlideInfoCollection.Clear();

            if (string.IsNullOrEmpty(currentSlidesPath))
                return;

            DirectoryInfo directory = new DirectoryInfo(currentSlidesPath);
            if (directory.Exists)
            {
                bool first = true;
                foreach (DirectoryInfo imageFolder in directory.EnumerateDirectories())
                {
                    if (!Container.Resolve<FileService>().GetImagesInFolder(imageFolder, approvedExtensions, true).Any())
                        continue;

                    SlideInfo slideInfo = new SlideInfo() { Category = "Folder", Name = imageFolder.Name };
                    SlideInfoCollection.Add(slideInfo);

                    if (first)
                    {
                        SelectedSlideInfo = slideInfo;
                        first = false;
                    }
                }

                foreach (FileInfo fileInfo in Container.Resolve<FileService>().GetImagesInFolder(directory, approvedExtensions, false))
                {
                    SlideInfo slideInfo = new SlideInfo() { Category = "File", Name = fileInfo.Name };
                    SlideInfoCollection.Add(slideInfo);

                    if (first)
                    {
                        SelectedSlideInfo = slideInfo;
                        first = false;
                    }
                }
            }
        }

        /// <summary>
        /// Previous 버튼 클릭 시
        /// </summary>
        private void PreviousSlide()
        {
            int idx = SlideInfoCollection.IndexOf(SelectedSlideInfo);
            if (idx <= 0)
                return;

            SelectedSlideInfo = SlideInfoCollection[idx - 1];
        }

        /// <summary>
        /// Next 버튼 클릭 시
        /// </summary>
        private void NextSlide()
        {
            int idx = SlideInfoCollection.IndexOf(SelectedSlideInfo);
            if (idx < 0 || idx >= SlideInfoCollection.Count - 1)
                return;

            SelectedSlideInfo = SlideInfoCollection[idx + 1];
        }

        /// <summary>
        /// 이미지 또는 동영상 슬라이드를 화면에 출력합니다.
        /// </summary>
        /// <param name="slideChanged"></param>
        private void DisplaySlide(bool slideChanged)
        {
            string slidePath = Path.Combine(currentSlidesPath, SelectedSlideInfo.Name);

            FileInfo file;
            if (SelectedSlideInfo.Category == "Folder")
                file = Container.Resolve<FileService>().FindImageInSlide(new DirectoryInfo(slidePath), approvedExtensions, 0, 0, 0, ZSSliderValue);
            else
                file = new FileInfo(slidePath);
               
            // 파일 실존하는지 확인
            if (file == null || !file.Exists)
            {
                StopSlideshow();
                WinUIMessageBox.Show("파일이 존재하지 않습니다.", "슬라이드 이동", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 표시중인 파일과 같은 이름이면 무시
            if (currentFile?.FullName == file.FullName)
                return;

            currentFile = file;

            // 메타 데이터 로드
            Metadata metadata = Container.Resolve<FileService>().ReadMetadataOfImage(currentSlidesPath, currentFile);

            // 디스플레이
            if (imageFileExtensions.Any(s => s.Equals(currentFile.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
                viewerPage = imagePage;
                EventAggregator.GetEvent<DisplayImageEvent>().Publish(new DisplayParam(currentFile, metadata, slideChanged));
            }
            else if (videoFileExtensions.Any(s => s.Equals(currentFile.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
                viewerPage = videoPage;
                EventAggregator.GetEvent<DisplayVideoEvent>().Publish(new DisplayParam(currentFile, metadata, slideChanged));
            }

            //DisplayImageWithMetadata(metadata);

            Container.Resolve<DataManager>().CurrentFile = currentFile;
            Container.Resolve<DataManager>().Metadata = metadata;
            Container.Resolve<DataManager>().SelectedSlideInfo = SelectedSlideInfo;
            Container.Resolve<DataManager>().ViewerPage = viewerPage;

            EventAggregator.GetEvent<ViewerPageChangedEvent>().Publish();
        }

        /// <summary>
        /// StopSlideshow
        /// </summary>
        private void StopSlideshow()
        {
            Container.Resolve<SlideShowService>().StopSlideShow();
            CurrentPlayingSlider = -1;
        }

        /// <summary>
        /// 현재 폴더에서의 슬라이드 목록을 새로고침 합니다.
        /// </summary>
        /// <param name="currentSlidesPath"></param>
        /// <param name="slideName"></param>
        private void EnableImageSliders(string currentSlidesPath, string slideName)
        {
            disableSlidersEvent = true;

            if (approvedExtensions.Any(s => slideName.EndsWith(s)))
            {
                ZSSliderValue = 0;
                ZSSliderMaximum = 0;
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(currentSlidesPath, slideName));
                var (tlCount, mpCount, msCount, zsCount) = Container.Resolve<FileService>().GetImagesModeStatus(di, approvedExtensions);

                ZSSliderMinimum = 1;
                ZSSliderMaximum = zsCount;
                ZSSliderValue = zsCount == 0 ? 0 : 1;
            }

            disableSlidersEvent = false;
        }

        /// <summary>
        /// 메타데이터 표출
        /// </summary>
        /// <param name="metadata"></param>
        private void DisplayImageWithMetadata(Metadata metadata)
        {
            MetadataCollection.Clear();

            if (metadata != null)
            {
                SelectedFilename = metadata.FileName;
                MetadataCollection = Container.Resolve<FileService>().ToModel(metadata);
            }
        }

        /// <summary>
        /// SlideShow Play
        /// </summary>
        /// <param name="type"></param>
        private void PlaySlideShow(string type)
        {
            switch (type)
            {
                case "ZStack":
                    Container.Resolve<SlideShowService>().StopSlideShow();

                    if (CurrentPlayingSlider != 0 && ZSSliderEnabled && ZSSliderMaximum >= 2)
                    {
                        Container.Resolve<SlideShowService>().StartSlideShow(SlideShowFps, ZSSliderMaximum - 1, SlideShowRepeat);

                        // 현재 슬라이드 값이 1이 아닌 경우: 1로 이동시키면 재생 시작
                        // 현재 슬라이드 값이 1인 경우: 이동시킬 필요 없으므로 바로 재생 시작
                        if (ZSSliderValue == 1)
                        {
                            if (viewerPage == imagePage)
                                Container.Resolve<SlideShowService>().ContinueSlideShow();
                            else
                                EventAggregator.GetEvent<PlayVideoEvent>().Publish();
                        }
                        else
                            ZSSliderValue = 1;

                        CurrentPlayingSlider = 0;
                    }
                    else
                        CurrentPlayingSlider = -1;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InternalPlaySlideshow()
        {
            switch (CurrentPlayingSlider)
            {
                case 0:
                    if (ZSSliderValue == ZSSliderMaximum)
                    {
                        ZSSliderValue = ZSSliderMinimum;
                    }
                    else
                    {
                        ZSSliderValue++;
                        if (!Container.Resolve<SlideShowService>().NowPlaying && ZSSliderValue == ZSSliderMaximum)
                            CurrentPlayingSlider = -1;
                    }
                    break;
            }
        }

        /// <summary>
        /// 회전 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void Rotation(string type)
        {
            EventAggregator.GetEvent<RotationEvent>().Publish(type);
        }

        /// <summary>
        /// 좌우 또는 상하 반전 이벤트
        /// </summary>
        /// <param name="type"></param>
        private void Reflect(string type)
        {
            EventAggregator.GetEvent<ReflectEvent>().Publish(type);
        }

        /// <summary>
        /// 회전, 반전 초기화
        /// </summary>
        private void RotationReset()
        {
            EventAggregator.GetEvent<RotationResetEvent>().Publish();
        }

        /// <summary>
        /// 슬라이더 상태 변경
        /// </summary>
        private void SliderStateChanged()
        {
            RaisePropertyChanged(nameof(ZSSliderEnabled));
        }
    }
}
