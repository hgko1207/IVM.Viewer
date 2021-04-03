using IVM.Studio.Models;
using IVM.Studio.MvvM;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Unity;
using static IVM.Studio.Models.Common;

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
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<FolderInfo> folderInfoList;
        public ObservableCollection<FolderInfo> FolderInfoList => folderInfoList ?? (folderInfoList = new ObservableCollection<FolderInfo>());

        private FolderInfo selectedFileInfo;
        public FolderInfo SelectedFileInfo
        {
            get => selectedFileInfo;
            set
            {
                if (SetProperty(ref selectedFileInfo, value) && value != null)
                {
                    //StopSlideshow();
                    //EnableImageSliders(currentFolderPath, value.Name);
                    //DisplaySlide(true);
                }
            }
        }

        public List<int> PenThicknessList { get; }
        public List<int> EraserSizeList { get; }
        public List<int> FontSizeList { get; }
        public List<FontItem> FontItemList { get; }

        private int selectedPenThickness;
        public int SelectedPenThickness
        {
            get => selectedPenThickness;
            set => SetProperty(ref selectedPenThickness, value);
        }

        public int selectedEraserSize;
        public int SelectedEraserSize
        {
            get => selectedEraserSize;
            set => SetProperty(ref selectedEraserSize, value);
        }

        public int selectedFontSize;
        public int SelectedFontSize
        {
            get => selectedFontSize;
            set => SetProperty(ref selectedFontSize, value);
        }

        public FontItem selectedFontItem;
        public FontItem SelectedFontItem
        {
            get => selectedFontItem;
            set => SetProperty(ref selectedFontItem, value);
        }

        public ICommand OpenFolderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand DisplayAllCommand { get; private set; }

        private string currentFolderPath;

        private readonly IEnumerable<string> imageFileExtensions;
        private readonly IEnumerable<string> videoFileExtensions;
        private IEnumerable<string> extensions => Enumerable.Concat(imageFileExtensions, videoFileExtensions);

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        /// <param name="regionManager"></param>
        public MainWindowViewModel(IContainerExtension container, IRegionManager regionManager) : base(container)
        {
            regionManager.RegisterViewWithRegion("ImageControlPage", typeof(ImageControl));

            OpenFolderCommand = new DelegateCommand(OpenFolder);
            RefreshCommand = new DelegateCommand(Refresh);
            DisplayAllCommand = new DelegateCommand(DisplayAll);

            imageFileExtensions = new[] { ".ivm" };
            videoFileExtensions = new[] { ".avi" };

            PenThicknessList = new List<int>();
            for (int i = 1; i <= 10; i++)
                PenThicknessList.Add(i);

            EraserSizeList = new List<int>();
            for (int i = 1; i <= 50; i++)
                EraserSizeList.Add(i);

            FontSizeList = new List<int>();
            for (int i = 1; i <= 100; i++)
                FontSizeList.Add(i);

            FontItemList = new List<FontItem>();
            FontItemList.Add(new FontItem() { Type = "1", Name = "맑은 고딕" });

            SelectedPenThickness = 1;
            SelectedEraserSize = 30;
            SelectedFontSize = 40;
            SelectedFontItem = FontItemList[0];
        }

        /// <summary>
        /// 폴더 열기
        /// </summary>
        private void OpenFolder()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (!string.IsNullOrEmpty(currentFolderPath))
                folderBrowserDialog.SelectedPath = currentFolderPath;

            if (folderBrowserDialog.ShowDialog().GetValueOrDefault())
                currentFolderPath = folderBrowserDialog.SelectedPath;

            Refresh();
        }

        /// <summary>
        /// 새로고침 이벤트
        /// </summary>
        private void Refresh()
        {
            FolderInfoList.Clear();

            if (string.IsNullOrEmpty(currentFolderPath))
                return;

            DirectoryInfo directory = new DirectoryInfo(currentFolderPath);
            if (!directory.Exists)
                return;

            bool first = true;
            foreach (DirectoryInfo imageFolder in directory.EnumerateDirectories())
            {
                if (!Container.Resolve<FileService>().GetImagesInFolder(imageFolder, extensions, true).Any())
                    continue;

                FolderInfo folderInfo = new FolderInfo() { Category = "Folder", Filename = imageFolder.Name };
                FolderInfoList.Add(folderInfo);

                if (first)
                {
                    SelectedFileInfo = folderInfo;
                    first = false;
                }
            }

            foreach (FileInfo fileInfo in Container.Resolve<FileService>().GetImagesInFolder(directory, extensions, false))
            {
                FolderInfo folderInfo = new FolderInfo() { Category = "File", Filename = fileInfo.Name };
                FolderInfoList.Add(folderInfo);

                if (first)
                {
                    SelectedFileInfo = folderInfo;
                    first = false;
                }
            }
        }

        /// <summary>
        /// Display 창 호출 이벤트
        /// </summary>
        private void DisplayAll()
        {
            Container.Resolve<WindowByChannelService>().ShowDisplay(0, true);
        }
    }
}
