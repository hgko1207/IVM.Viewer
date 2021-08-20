using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System;
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
        I3DWcfServer wcfserver;

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

        private int openSliceLower = 0;
        public int OpenSliceLower
        {
            get => openSliceLower;
            set
            {
                if (SetProperty(ref openSliceLower, value))
                {
                    OpenSliceText = string.Format("{0} / {1}", openSliceLimit - openSliceLower - (openSliceLimit - openSliceUpper), openSliceLimit);
                }
            }
        }

        private int openSliceUpper = 1000;
        public int OpenSliceUpper
        {
            get => openSliceUpper;
            set
            {
                if (SetProperty(ref openSliceUpper, value))
                {
                    OpenSliceText = string.Format("{0} / {1}", openSliceLimit - openSliceLower - (openSliceLimit - openSliceUpper), openSliceLimit);
                }
            }
        }

        private int openSliceLimit = 1000;
        public int OpenSliceLimit
        {
            get => openSliceLimit;
            set => SetProperty(ref openSliceLimit, value);
        }

        private string openSliceText = "";
        public string OpenSliceText
        {
            get => openSliceText;
            set => SetProperty(ref openSliceText, value);
        }

        private bool openReverse = false;
        public bool OpenReverse
        {
            get => openReverse;
            set => SetProperty(ref openReverse, value);
        }

        public ICommand OpenFolderCommand { get; private set; }
        public ICommand OpenSelectedCommand { get; private set; }
        public ICommand ChangedSelectedCommand { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DImportPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();

            OpenFolderCommand = new DelegateCommand(OpenFolder);
            OpenSelectedCommand = new DelegateCommand(OpenSelected);
            ChangedSelectedCommand = new DelegateCommand(ChangedSelected);
        }

        private void ChangedSelected()
        {
            OpenSliceLimit = SelectedImgInfo.Count;

            OpenSliceLower = 0;
            OpenSliceUpper = OpenSliceLimit;
        }

        private int CalcTextureCountInDirectory(string imgPath)
        {
            string[] files = Directory.GetFiles(imgPath).OrderBy(f => f).ToArray();

            int cnt = 0;

            foreach (string imgpath in files)
            {
                string ext = Path.GetExtension(imgpath).ToLower();
                if (!(new string[] { ".tif", ".png" }).Contains(ext))
                    continue;

                cnt++;
            }

            return cnt;
        }

        private int CalcTextureCount(string imgPath, ref bool is4D)
        {
            int cnt = CalcTextureCountInDirectory(imgPath);

            if (cnt <= 0)
            {
                string[] dirs = Directory.GetDirectories(imgPath).OrderBy(f => f).ToArray();

                foreach (string d in dirs)
                {
                    int cnt2 = CalcTextureCountInDirectory(d);
                    if (cnt2 > 0)
                    {
                        is4D = true;
                        return cnt2;
                    }
                }
            }
            
            is4D = false;
            return cnt;
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
                    bool is4D = false;
                    int cnt = CalcTextureCount(dir, ref is4D);

                    I3DPathInfo pathInfo = new I3DPathInfo() { Path = dir, Count = cnt, Dim = (is4D ? "4D" : "3D") };
                    ImgPathCollection.Add(pathInfo);
                }
            }
        }

        private void OpenSelected()
        {
            if (SelectedImgInfo == null)
                return;

            CurrentImgPath = SelectedImgInfo.Path;

            if (!Directory.Exists(CurrentImgPath))
                return;

            wcfserver.channel1.OnOpen(CurrentImgPath, OpenSliceLower, OpenSliceUpper, OpenReverse);
            wcfserver.channel2.OnOpen(CurrentImgPath, OpenSliceLower, OpenSliceUpper, OpenReverse);
        }
    }
}
