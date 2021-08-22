using DevExpress.Xpf.Docking;
using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Threading;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DFreeRecordingPanelViewModel : ViewModelBase
    {
        I3DWcfServer wcfserver;

        DataManager datamanager;
        public I3DRecordInfo I3DRecordInfo { get; set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DFreeRecordingPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();
            datamanager = container.Resolve<DataManager>();

            I3DRecordInfo = datamanager.I3DRecordInfo;
        }
    }
}
