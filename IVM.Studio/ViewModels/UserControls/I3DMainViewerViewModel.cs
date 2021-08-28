using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Events;
using Prism.Ioc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DMainViewerViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<I3DMainViewer>
    {
        private I3DMainViewer view;
        I3DWcfServer wcfserver;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DMainViewerViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();
        }

        public void OnLoaded(I3DMainViewer view)
        {
            this.view = view;

            EventAggregator.GetEvent<I3DCameraUpdateEvent>().Subscribe(UpdateCamera);
        }

        public void OnUnloaded(I3DMainViewer view)
        {
            EventAggregator.GetEvent<I3DCameraUpdateEvent>().Unsubscribe(UpdateCamera);
        }

        private void UpdateCamera(I3DCameraUpdateParam p)
        {
            if (p.Viewtype == (int)I3DViewType.MAIN_VIEW)
                return;

            wcfserver.channel1.OnUpdateCamera(p.px, p.py, p.pz, p.ax, p.ay, p.az, p.s);
        }
    }
}
