using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DImportPanelViewModel : ViewModelBase
    {
        public ICommand OpenFolderCommand { get; private set; }

        private string currentImgPath;

        public string CurrentImgPath
        {
            get => currentImgPath;
            set => SetProperty(ref currentImgPath, value);
        }

        private ObservableCollection<I3DPathInfo> imgPathCollection = new ObservableCollection<I3DPathInfo>();
        public ObservableCollection<I3DPathInfo> ImgPathCollection
        {
            get => imgPathCollection;
            set => SetProperty(ref imgPathCollection, value);
        }

        private I3DPathInfo selectedImgInfo;
        public I3DPathInfo SelectedImgInfo
        {
            get => selectedImgInfo;
            set => SetProperty(ref selectedImgInfo, value);
        }

        public ICommand OpenSelectedCommand { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DImportPanelViewModel(IContainerExtension container) : base(container)
        {
            OpenFolderCommand = new DelegateCommand(OpenFolder);
            OpenSelectedCommand = new DelegateCommand(OpenSelected);
        }

        private void OpenFolder()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (!string.IsNullOrEmpty(CurrentImgPath))
                folderBrowserDialog.SelectedPath = CurrentImgPath;

            if (folderBrowserDialog.ShowDialog().GetValueOrDefault())
                CurrentImgPath = folderBrowserDialog.SelectedPath;

            if (CurrentImgPath != null)
            {
                string[] dirs = Directory.GetDirectories(CurrentImgPath).OrderBy(f => f).ToArray();

                foreach (string dir in dirs)
                {
                    I3DPathInfo pathInfo = new I3DPathInfo() { Path = dir };
                    ImgPathCollection.Add(pathInfo);
                }

                //EventAggregator.GetEvent<I3DOpenEvent>().Publish(CurrentImgPath);
            }
        }

        private void OpenSelected()
        {
            CurrentImgPath = SelectedImgInfo.Path;

            EventAggregator.GetEvent<I3DOpenEvent>().Publish(CurrentImgPath);
        }
    }
}
