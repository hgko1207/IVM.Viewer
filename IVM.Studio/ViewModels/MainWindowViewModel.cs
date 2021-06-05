using DevExpress.Xpf.WindowsUI;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.MvvM;
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
using System.Windows.Input;
using Unity;

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

        public ObservableCollection<MetadataModel> MetadataCollection = new ObservableCollection<MetadataModel>();

        

        public ICommand OpenFolderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand PreviousSlideCommand { get; private set; }
        public ICommand NextSlideCommand { get; private set; }

        public ICommand RotationCommand { get; private set; }
        public ICommand ReflectCommand { get; private set; }

        private string currentSlidesPath;

        private readonly IEnumerable<string> imageFileExtensions;
        private readonly IEnumerable<string> videoFileExtensions;
        private IEnumerable<string> extensions => Enumerable.Concat(imageFileExtensions, videoFileExtensions);

        private FileInfo currentFile;

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

            OpenFolderCommand = new DelegateCommand(OpenFolder);
            RefreshCommand = new DelegateCommand(Refresh);
            PreviousSlideCommand = new DelegateCommand(PreviousSlide);
            NextSlideCommand = new DelegateCommand(NextSlide);

            RotationCommand = new DelegateCommand<string>(Rotation);
            ReflectCommand = new DelegateCommand<string>(Reflect);

            imageFileExtensions = new[] { ".ivm" };
            videoFileExtensions = new[] { ".avi" };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(MainWindow view)
        {
        }

        public void OnUnloaded(MainWindow view)
        {
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
            if (!directory.Exists)
                return;

            bool first = true;
            foreach (DirectoryInfo imageFolder in directory.EnumerateDirectories())
            {
                if (!Container.Resolve<FileService>().GetImagesInFolder(imageFolder, extensions, true).Any())
                    continue;

                SlideInfo slideInfo = new SlideInfo() { Category = "Folder", Name = imageFolder.Name };
                SlideInfoCollection.Add(slideInfo);

                if (first)
                {
                    SelectedSlideInfo = slideInfo;
                    first = false;
                }
            }

            foreach (FileInfo fileInfo in Container.Resolve<FileService>().GetImagesInFolder(directory, extensions, false))
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

            FileInfo file = null;
            if (SelectedSlideInfo.Category == "File")
                file = new FileInfo(slidePath);
            else
                file = Container.Resolve<FileService>().FindImageInSlide(new DirectoryInfo(slidePath), extensions, 0, 0, 0, 0);

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
            if (extensions.Any(s => s.Equals(currentFile.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {
                DisplayImageWithMetadata(metadata);
            }
            else if (extensions.Any(s => s.Equals(currentFile.Extension, StringComparison.InvariantCultureIgnoreCase)))
            {

            }
        }

        private void StopSlideshow()
        {
        }

        private void EnableImageSliders(string currentSlidesPath, string name)
        {
        }

        private void DisplayImageWithMetadata(Metadata metadata)
        {
            MetadataCollection.Clear();

            if (metadata != null)
            {
                MetadataCollection.Add(new MetadataModel() { Group = "Filename", Name1 = metadata.FileName, Value1 = metadata.FileName });
            }
        }

        /// <summary>
        /// 회전
        /// </summary>
        /// <param name="type"></param>
        private void Rotation(string type)
        {
            switch (type)
            {
                case "Left":
                    break;
                case "Right":
                    break;
            }
        }

        /// <summary>
        /// 전환
        /// </summary>
        /// <param name="type"></param>
        private void Reflect(string type)
        {
            switch (type)
            {
                case "HorizontalLeft":
                    break;
                case "HorizontalRight":
                    break;
                case "VerticalTop":
                    break;
                case "VerticalBottom":
                    break;
            }
        }
    }
}
