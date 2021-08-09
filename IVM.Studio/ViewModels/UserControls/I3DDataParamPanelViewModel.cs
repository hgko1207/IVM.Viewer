using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Ioc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DDataParamPanelViewModel : ViewModelBase
    {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DDataParamPanelViewModel(IContainerExtension container) : base(container)
        {
        }
    }
}
