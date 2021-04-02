using IVM.Studio.Models;
using IVM.Studio.MvvM;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Unity;

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

        public ICommand OpenFolderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

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

            imageFileExtensions = new[] { ".ivm" };
            videoFileExtensions = new[] { ".avi" };
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
    }
}
