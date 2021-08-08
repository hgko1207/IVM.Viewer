using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
using System.Windows.Input;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DImportPanelViewModel : ViewModelBase
    {
        public ICommand OpenFolderCommand { get; private set; }

        private string currentSlidesPath;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DImportPanelViewModel(IContainerExtension container) : base(container)
        {
            OpenFolderCommand = new DelegateCommand(OpenFolder);
        }

        private void OpenFolder()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            if (!string.IsNullOrEmpty(currentSlidesPath))
                folderBrowserDialog.SelectedPath = currentSlidesPath;

            if (folderBrowserDialog.ShowDialog().GetValueOrDefault())
                currentSlidesPath = folderBrowserDialog.SelectedPath;

            EventAggregator.GetEvent<I3DOpenEvent>().Publish(currentSlidesPath);
        }
    }
}
