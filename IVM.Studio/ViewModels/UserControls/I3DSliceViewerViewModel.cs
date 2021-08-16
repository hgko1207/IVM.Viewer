using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DSliceViewerViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<I3DSliceViewer>
    {
        private I3DSliceViewer view;
        I3DWcfServer wcfserver;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DSliceViewerViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();
        }

        public void OnLoaded(I3DSliceViewer view)
        {
            this.view = view;

            EventAggregator.GetEvent<I3DOpenEvent>().Subscribe(Open, ThreadOption.UIThread);
            EventAggregator.GetEvent<I3DCameraUpdateEvent>().Subscribe(UpdateCamera);
        }

        public void OnUnloaded(I3DSliceViewer view)
        {
            EventAggregator.GetEvent<I3DOpenEvent>().Unsubscribe(Open);
            EventAggregator.GetEvent<I3DCameraUpdateEvent>().Unsubscribe(UpdateCamera);
        }

        private void Open(string path)
        {
            wcfserver.channel2.OnOpen(path);
        }
        private void UpdateCamera(I3DCameraUpdateParam p)
        {
            if (p.viewtype == (int)I3DViewType.SLICE_VIEW)
                return;

            wcfserver.channel2.OnUpdateCamera(p.px, p.py, p.pz, p.ax, p.ay, p.az, p.s);
        }
    }
}
