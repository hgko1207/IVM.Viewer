using IVM.Studio.Models;
using IVM.Studio.MvvM;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Unity;

namespace IVM.Studio.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<FolderInfo> folderInfoList;
        public ObservableCollection<FolderInfo> FolderInfoList => folderInfoList ?? (folderInfoList = new ObservableCollection<FolderInfo>());

        public ICommand OpenFolderCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }

        private string currentFolderPath;

        public MainWindowViewModel()
        {
            OpenFolderCommand = new DelegateCommand(OpenFolder);
            RefreshCommand = new DelegateCommand(Refresh);
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
                string folderName = imageFolder.Name;
            }
        }
    }
}
