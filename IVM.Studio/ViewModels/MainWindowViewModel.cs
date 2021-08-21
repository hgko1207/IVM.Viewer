using DevExpress.Xpf.WindowsUI;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Models.Views;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using IVM.Studio.Views.UserControls;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
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

        /// <summary>
        /// ImageList 테이블에서 선택 시
        /// </summary>
        private SlideInfo selectedSlideInfo;
        public SlideInfo SelectedSlideInfo
        {
            get => selectedSlideInfo;
            set
            {
                if (SetProperty(ref selectedSlideInfo, value) && value != null)
                {
                    EventAggregator.GetEvent<StopSlideShowEvent>().Publish();
                    EventAggregator.GetEvent<EnableImageSlidersEvent>().Publish(new SlidersParam() { CurrentSlidesPath = currentSlidesPath, SlideName = value.Name });
                    DisplaySlide(true);
                    dataManager.SelectedSlideInfo = value;

                    RaisePropertyChanged(nameof(WindowOpenEnabled));
                }
            }
        }

        public bool WindowOpenEnabled => SelectedSlideInfo != null;

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

        private bool checked2DMode = true;
        public bool Checked2DMode
        {
            get => checked2DMode;
            set => SetProperty(ref checked2DMode, value);
        }

        private bool checked3DMode = false;
        public bool Checked3DMode
        {
            get => checked3DMode;
            set => SetProperty(ref checked3DMode, value);
        }

        private Visibility visUI2D = Visibility.Visible;
        public Visibility VisUI2D
        {
            get => visUI2D;
            set => SetProperty(ref visUI2D, value);
        }

        private Visibility visUI3D = Visibility.Hidden;
        public Visibility VisUI3D
        {
            get => visUI3D;
            set => SetProperty(ref visUI3D, value);
        }

        public ICommand OpenFolderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand WindowOpenCommand { get; private set; }
        public ICommand Change2DModeCommand { get; private set; }
        public ICommand Change3DModeCommand { get; private set; }

        private MainWindow mainWindow; // entry MainWindow

        // for I3D Viewers
        private WindowSnapper snapper1; // 3d-main-view
        private WindowSnapper snapper2; // 3d-slice-view
        private I3DWcfServer wcfserver;

        private readonly IEnumerable<string> imageFileExtensions;
        private readonly IEnumerable<string> videoFileExtensions;
        private IEnumerable<string> approvedExtensions => Enumerable.Concat(imageFileExtensions, videoFileExtensions);

        private string currentSlidesPath;
        private FileInfo currentFile;
        private DirectoryInfo currentDirectory;

        private DataManager dataManager;

        public SliderControlInfo SliderControlInfo { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public MainWindowViewModel(IContainerExtension container) : base(container)
        {
            RegionManager.RegisterViewWithRegion("ImageAdjustmentPanel", typeof(ImageAdjustmentPanel));
            RegionManager.RegisterViewWithRegion("BrightnessPanel", typeof(BrightnessPanel));
            RegionManager.RegisterViewWithRegion("AnnotationPanel", typeof(AnnotationPanel));
            RegionManager.RegisterViewWithRegion("ColormapPanel", typeof(ColormapPanel));
            RegionManager.RegisterViewWithRegion("DisplayControlPanel", typeof(DisplayControlPanel));
            //RegionManager.RegisterViewWithRegion("ThumbnailPanel", typeof(ThumbnailPanel));
            RegionManager.RegisterViewWithRegion("ChannelProcessingPanel", typeof(ChannelProcessingPanel));
            RegionManager.RegisterViewWithRegion("MeasurementPanel", typeof(MeasurementPanel));
            RegionManager.RegisterViewWithRegion("PostProcessingPanel", typeof(PostProcessingPanel));
            RegionManager.RegisterViewWithRegion("RotateCropPanel", typeof(RotateCropPanel));

            // 3D-Panels
            RegionManager.RegisterViewWithRegion("I3DImportPanel", typeof(I3DImportPanel));
            RegionManager.RegisterViewWithRegion("I3DDataParamPanel", typeof(I3DDataParamPanel));
            RegionManager.RegisterViewWithRegion("I3D3DViewPanel", typeof(I3D3DViewPanel));
            RegionManager.RegisterViewWithRegion("I3DSliceViewPanel", typeof(I3DSliceViewPanel));
            RegionManager.RegisterViewWithRegion("I3D3DDisplayPanel", typeof(I3D3DDisplayPanel));
            RegionManager.RegisterViewWithRegion("I3DMainViewer", typeof(I3DMainViewer));
            RegionManager.RegisterViewWithRegion("I3DSliceViewer", typeof(I3DSliceViewer));
            RegionManager.RegisterViewWithRegion("I3DFreeRecordingPanel", typeof(I3DFreeRecordingPanel));
            RegionManager.RegisterViewWithRegion("I3DSceneRecordingPanel", typeof(I3DSceneRecordingPanel));

            dataManager = container.Resolve<DataManager>();
            dataManager.Init(container, EventAggregator);

            OpenFolderCommand = new DelegateCommand(OpenFolder);
            RefreshCommand = new DelegateCommand(Refresh);
            WindowOpenCommand = new DelegateCommand(WindowOpen);

            // 2D / 3D Mode Change
            Change2DModeCommand = new DelegateCommand(Change2DMode);
            Change3DModeCommand = new DelegateCommand(Change3DMode);

            EventAggregator.GetEvent<DisplaySlideEvent>().Subscribe(DisplaySlide);
            EventAggregator.GetEvent<RefreshMetadataEvent>().Subscribe(DisplayImageWithMetadata, ThreadOption.UIThread);
            EventAggregator.GetEvent<RefreshFolderEvent>().Subscribe(RefreshFolder);
            
            EventAggregator.GetEvent<I3DWindowLoadedEvent>().Subscribe(I3DWindowLoaded);
            EventAggregator.GetEvent<I3DMainViewVisibleChangedEvent>().Subscribe(I3DMainViewVisibleChanged);
            EventAggregator.GetEvent<I3DSliceViewVisibleChangedEvent>().Subscribe(I3DSliceViewVisibleChanged);


            imageFileExtensions = new[] { ".ivm" };
            videoFileExtensions = new[] { ".avi" };

            SliderControlInfo = dataManager.SliderControlInfo;

            // I3DViewer Remote Communication
            wcfserver = container.Resolve<I3DWcfServer>();
            wcfserver.Init(EventAggregator);
            wcfserver.Listen();
        }

        private void I3DMainViewVisibleChanged(bool v)
        {
            if (v)
                snapper1.Show();
            else
                snapper1.Hide();
        }

        private void I3DSliceViewVisibleChanged(bool v)
        {
            if (v)
                snapper2.Show();
            else
                snapper2.Hide();
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(MainWindow view)
        {
            mainWindow = view;

            // init mainwindow event-instance
            mainWindow.EventAggregator = EventAggregator;

            // create I3D windows
            snapper1 = new WindowSnapper(mainWindow, mainWindow.i3dmv, "I3D_MAIN_VIEW", @".\I3D\IVM.I3DApp.exe");
            snapper2 = new WindowSnapper(mainWindow, mainWindow.i3dsv, "I3D_SLICE_VIEW", @".\I3D\IVM.I3DApp.exe");
            mainWindow.i3dmv.Loaded += I3dmv_Loaded;
            mainWindow.i3dsv.Loaded += I3dsv_Loaded;
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(MainWindow view)
        {
            EventAggregator.GetEvent<DisplaySlideEvent>().Unsubscribe(DisplaySlide);
            EventAggregator.GetEvent<RefreshMetadataEvent>().Unsubscribe(DisplayImageWithMetadata);
            EventAggregator.GetEvent<RefreshFolderEvent>().Unsubscribe(RefreshFolder);

            EventAggregator.GetEvent<I3DWindowLoadedEvent>().Unsubscribe(I3DWindowLoaded);
            EventAggregator.GetEvent<I3DMainViewVisibleChangedEvent>().Unsubscribe(I3DMainViewVisibleChanged);
            EventAggregator.GetEvent<I3DSliceViewVisibleChangedEvent>().Unsubscribe(I3DSliceViewVisibleChanged);

            // kill I3D windows
            snapper1.KillProcess();
            snapper2.KillProcess();
        }

        private void I3dmv_Loaded(object sender, RoutedEventArgs e)
        {
            snapper1.InvokeProcess();
        }

        private void I3dsv_Loaded(object sender, RoutedEventArgs e)
        {
            snapper2.InvokeProcess();
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

                    SlideInfo slideInfo = new SlideInfo() { Category = "Folder", Name = imageFolder.Name, Format = FolderNameToFormat(imageFolder.Name) };
                    SlideInfoCollection.Add(slideInfo);

                    if (first)
                    {
                        SelectedSlideInfo = slideInfo;
                        first = false;
                    }
                }

                foreach (FileInfo fileInfo in Container.Resolve<FileService>().GetImagesInFolder(directory, approvedExtensions, false))
                {
                    string format = imageFileExtensions.Any(s => s.Equals(fileInfo.Extension, StringComparison.InvariantCultureIgnoreCase)) ? "PNG" : "AVI";

                    SlideInfo slideInfo = new SlideInfo() { Category = "File", Name = fileInfo.Name, Format = format };
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
        /// 폴더 명을 통해 포맷 찾기
        /// </summary>
        /// <param name="forderName"></param>
        /// <returns></returns>
        private string FolderNameToFormat(string forderName)
        {
            string format = "";

            string[] names = forderName.Split('-');
            for (int i = 0; i < names.Length; i++)
            {
                if (i + 1 == names.Length)
                {
                    format += names[i].Substring(0, 2);
                }
                else
                {
                    format += names[i] + "_";
                }
            }

            return format;
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
            {
                currentDirectory = new DirectoryInfo(slidePath);
                file = Container.Resolve<FileService>().FindImageInSlide(currentDirectory, approvedExtensions,
                   SliderControlInfo.TLSliderValue, SliderControlInfo.MPSliderValue, SliderControlInfo.MSSliderValue, SliderControlInfo.ZSSliderValue);
            }
            else
            {
                currentDirectory = null;
                file = new FileInfo(slidePath);
            }

            // 파일 실존하는지 확인
            if (file == null || !file.Exists)
            {
                EventAggregator.GetEvent<StopSlideShowEvent>().Publish();
                WinUIMessageBox.Show("파일이 존재하지 않습니다.", "슬라이드 이동", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 표시중인 파일과 같은 이름이면 무시
            if (currentFile?.FullName == file.FullName)
                return;

            currentFile = file;

            // 메타 데이터 로드
            Metadata metadata = Container.Resolve<FileService>().ReadMetadataOfImage(currentSlidesPath, currentFile);

            DisplayParam displayParam = new DisplayParam(currentFile, metadata, slideChanged);
            EventAggregator.GetEvent<RefreshMetadataEvent>().Publish(displayParam);

            // 디스플레이
            if (imageFileExtensions.Any(s => s.Equals(currentFile.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
                dataManager.ViewerName = nameof(ImageViewer);
                EventAggregator.GetEvent<DisplayImageEvent>().Publish(displayParam);
            }
            else if (videoFileExtensions.Any(s => s.Equals(currentFile.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
                dataManager.ViewerName = nameof(VideoViewer);
                EventAggregator.GetEvent<DisplayVideoEvent>().Publish(displayParam);
            }

            dataManager.CurrentSlidesPath = currentSlidesPath;
            dataManager.CurrentFile = currentFile;
            dataManager.Metadata = metadata;

            if (slideChanged)
                EventAggregator.GetEvent<ViewerPageChangeEvent>().Publish();
        }

        /// <summary>
        /// Window Open
        /// </summary>
        private void WindowOpen()
        {
            WindowInfo windowInfo = new WindowInfo() { Name = SelectedSlideInfo.Name, Seq = ++dataManager.MainWindowSeq, DirectoryInfo = currentDirectory };

            MainViewerWindow mainViewerWindow = new MainViewerWindow(windowInfo) { Owner = Application.Current.MainWindow };
            mainViewerWindow.Show();

            if (currentFile != null)
            {
                Metadata metadata = dataManager.Metadata;
                if (dataManager.ViewerName == nameof(ImageViewer))
                    EventAggregator.GetEvent<DisplayImageEvent>().Publish(new DisplayParam(currentFile, metadata, true));
                else
                    EventAggregator.GetEvent<DisplayVideoEvent>().Publish(new DisplayParam(currentFile, metadata, true));
            }
        }

        /// <summary>
        /// Change 2DMode
        /// </summary>
        private void Change2DMode()
        {
            Checked2DMode = true;
            Checked3DMode = false;

            VisUI2D = Visibility.Visible;
            VisUI3D = Visibility.Hidden;

            snapper1.Hide();
            snapper2.Hide();
        }

        /// <summary>
        /// Change 3DMode
        /// </summary>
        private void Change3DMode()
        {
            Checked2DMode = false;
            Checked3DMode = true;

            VisUI2D = Visibility.Hidden;
            VisUI3D = Visibility.Visible;

            snapper1.Show();
            snapper2.Show();
        }

        /// <summary>
        /// 메타데이터 표출
        /// </summary>
        /// <param name="param"></param>
        private void DisplayImageWithMetadata(DisplayParam param)
        {
            MetadataCollection.Clear();

            SelectedFilename = currentFile?.Name;

            Metadata metadata = param.Metadata;
            if (metadata != null)
                MetadataCollection = Container.Resolve<FileService>().ToModel(metadata);
        }

        /// <summary>
        /// Refresh Folder
        /// </summary>
        private void RefreshFolder(DirectoryInfo folder)
        {
            RefreshCommand.Execute(null);
            SelectedSlideInfo = SlideInfoCollection.FirstOrDefault(s => s.Name == folder?.Name && s.Category == "Folder");
        }

        private void I3DWindowLoaded(int viewtype)
        {
            // cross connection
            wcfserver.Connect(viewtype);

            // 3d-viewer windows attach
            if (viewtype == (int)I3DViewType.MAIN_VIEW)
            {
                snapper1.UpdateHandle();
                snapper1.Attach();
                snapper1.Hide();
            }
            else
            {
                snapper2.UpdateHandle();
                snapper2.Attach();
                snapper2.Hide();
            }
        }
    }
}
