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
using ImageRegistration;
using static IVM.Studio.Models.Common;
using MathWorks.MATLAB.NET.Arrays;
using System.Windows;
using System.Threading.Tasks;
using IVM.Studio.Views;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DImportPanelViewModel : ViewModelBase
    {
        I3DWcfServer wcfserver;

        ImageRegistration.ImageRegistration imgreg = null;

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

        private bool openAlignment = false;
        public bool OpenAlignment
        {
            get => openAlignment;
            set => SetProperty(ref openAlignment, value);
        }

        private string openAlignmentRef = "DAPI";
        public string OpenAlignmentRef
        {
            get => openAlignmentRef;
            set => SetProperty(ref openAlignmentRef, value);
        }

        public ICommand OpenFolderCommand { get; private set; }
        public ICommand OpenSelectedCommand { get; private set; }
        public ICommand ChangedSelectedCommand { get; private set; }
        public ICommand RunAlignmentCommand { get; private set; }
        

        int StrToBandIdx(string b)
        {
            switch (b)
            {
                case "DAPI":
                    return 1;
                case "GFP":
                    return 2;
                case "RFP":
                    return 3;
                case "NIR":
                    return 4;
            }
            return -1;
        }

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
            RunAlignmentCommand = new DelegateCommand(InvokeRunAlignment);
        }

        private void InvokeRunAlignment()
        {
            RunAlignment();
        }

        private bool RunAlignment()
        {
            if (imgreg == null)
            {
                try
                {
                    imgreg = new ImageRegistration.ImageRegistration();
                }
                catch (Exception e)
                {
                    MessageBox.Show("[ERROR] ImageRegistration failed\r\n\r\n" +
                        "Matlab-Runtime 을 설치해야 합니다. " +
                        "소프트웨어 설치 경로에 있는 MATLAB/ImageRegistration.MatlabRuntime.WebInstaller.exe 설치 파일을 실행해 주세요");
                    return false;
                }
            }

            if (SelectedImgInfo == null)
                return false;

            CurrentImgPath = SelectedImgInfo.Path;

            if (!Directory.Exists(CurrentImgPath))
                return false;

            string atag = "_ALIGN";

            MWArray reg_dir = CurrentImgPath;
            MWArray refCh = StrToBandIdx(OpenAlignmentRef);
            MWArray GPU = 0;
            MWArray save_dir = (CurrentImgPath + atag);

            LoadingWindow loading = new LoadingWindow();
            
            Task.Run(() =>
            {
                try
                {
                    imgreg.ZS_ShiftCorrected_array_IVIM(1, reg_dir, refCh, GPU, save_dir);
                    loading.loading = false;
                }
                catch (Exception e)
                {
                    loading.loading = false;
                    MessageBox.Show("[ERROR] ZS_ShiftCorrected_array_IVIM failed\r\n" + e.Message);
                }
            });

            loading.ShowDialog();

            return true;
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

                    string atag = "_ALIGN";
                    if (dir.IndexOf(atag) == (dir.Length - atag.Length))
                        continue;

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

            string atag = "";

            if (OpenAlignment)
            {
                atag = "_ALIGN";

                if (!Directory.Exists(CurrentImgPath + atag))
                {
                    if (RunAlignment() == false)
                        return;
                }
            }

            wcfserver.channel1.OnOpen(CurrentImgPath + atag, OpenSliceLower, OpenSliceUpper, OpenReverse);
            wcfserver.channel2.OnOpen(CurrentImgPath + atag, OpenSliceLower, OpenSliceUpper, OpenReverse);
        }
    }
}
