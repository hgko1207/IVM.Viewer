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

            EventAggregator.GetEvent<I3DOpenEvent>().Subscribe(Open, ThreadOption.UIThread);
        }

        public void OnUnloaded(I3DMainViewer view)
        {
            EventAggregator.GetEvent<I3DOpenEvent>().Unsubscribe(Open);
        }

        private void Open(string path)
        {
            wcfserver.channel.OnOpen(path);
        }
    }
}
